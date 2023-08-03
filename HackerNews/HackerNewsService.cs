using HackerNews.Core;
using HackerNews.Core.Model;
using HackerNews.Core.Model.Front;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.Json;

namespace HackerNews
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;
        private readonly HackerNewsApiSettings _settings;

        public HackerNewsService(IHttpClientFactory clientFactory, IMemoryCache cache, HackerNewsApiSettings settings)
        {
            _client = clientFactory.CreateClient();
            _cache = cache;
            _settings = settings;
        }

        public async Task RefreshBestStoriesCache(CancellationToken ct = default)
        {
            var storyIds = await GetBestStoriesIds(ct);
            var batchSize = 20;
            var storyBag = new ConcurrentBag<StoryResponse>();

            foreach (var batch in storyIds.Chunk(batchSize))
            {
                var tasks = batch.Select(id => GetStory(id));
                var stories = await Task.WhenAll(tasks);

                foreach (var story in stories)
                    storyBag.Add(story);
            }

            var orderedStories = storyBag.OrderByDescending(s => s.Score).ToList();
            _cache.Set(_settings.CacheKey, orderedStories, TimeSpan.FromMinutes(_settings.CacheLifetimeMinutes));
        }

        public async ValueTask<IEnumerable<StoryResponse>> GetBestStories(int limit, CancellationToken ct = default)
        {
            try
            {
                var besttories = _cache.Get<IEnumerable<StoryResponse>>(_settings.CacheKey);
                if (besttories is null || !besttories.Any()) //Cover the case of first execution or if while updating the cache API service was unavailable
                {
                    await RefreshBestStoriesCache(ct);
                    return _cache.Get<IEnumerable<StoryResponse>>(_settings.CacheKey);
                }

                return besttories.Take(limit); // as we only have 200 best stories coming from api this solution is good enough
            }
            catch (Exception)
            {
                return null;
            }

        }

        private async Task<IEnumerable<int>> GetBestStoriesIds(CancellationToken ct = default)
        {
            IEnumerable<int> stories = new List<int>();

            try
            {
                var response = await _client.GetAsync($"{_settings.BaseAddress}/beststories.json", ct);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to retrieve Best Stories. Status Code: {response.StatusCode}");


                var responseBody = await response.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(responseBody))
                    throw new Exception("Response content was null or empty.");

                stories = JsonSerializer.Deserialize<IEnumerable<int>>(responseBody);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return stories;
        }

        private async Task<StoryResponse> GetStory(int id, CancellationToken ct = default)
        {
            try
            {
                var response = await _client.GetAsync($"{_settings.BaseAddress}/item/{id}.json", ct);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to retrieve story with ID {id}. Status Code: {response.StatusCode}");

                var responseBody = await response.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(responseBody))
                    return null;

                var story = JsonSerializer.Deserialize<Story>(responseBody);
                var storyResponse = StoryMapper.MapToStoryResponse(story);
                return storyResponse;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
