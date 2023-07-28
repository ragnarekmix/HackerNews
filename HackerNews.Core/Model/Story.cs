using System.Text.Json.Serialization;

namespace HackerNews.Core.Model
{
    public class Story
    {
        [JsonPropertyName("by")]
        public string PostedBy { get; set; }

        [JsonPropertyName("descendants")]
        public int CommentCount { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("kids")]
        public List<long> Kids { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public string Uri { get; set; }
    }
}
