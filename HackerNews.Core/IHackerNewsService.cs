using HackerNews.Core.Model.Front;

namespace HackerNews.Core
{
    public interface IHackerNewsService
    {
        Task RefreshBestStoriesCache(CancellationToken ct = default);
        ValueTask<IEnumerable<StoryResponse>> GetBestStories(int limit, CancellationToken ct = default);
    }
}
