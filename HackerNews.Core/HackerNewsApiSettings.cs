namespace HackerNews.Core
{
    public class HackerNewsApiSettings
    {
        public string BaseAddress { get; set; }
        public int CacheLifetimeMinutes { get; set; }
        public string CacheKey { get; set; }
    }
}
