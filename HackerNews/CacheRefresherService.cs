using HackerNews.Core;
using Microsoft.Extensions.Hosting;

namespace HackerNews
{
    public class CacheRefresherService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IHackerNewsService _hackerNewsService;
        private readonly HackerNewsApiSettings _settings;

        public CacheRefresherService(IHackerNewsService hackerNewsService, HackerNewsApiSettings settings)
        {
            _hackerNewsService = hackerNewsService;
            _settings = settings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RefreshCache, null, TimeSpan.Zero, TimeSpan.FromMinutes(_settings.CacheLifetimeMinutes - 1));
            return Task.CompletedTask;
        }

        private async void RefreshCache(object state)
        {
            await _hackerNewsService.RefreshBestStoriesCache();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
