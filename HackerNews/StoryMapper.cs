using HackerNews.Core.Model;
using HackerNews.Core.Model.Front;

namespace HackerNews
{
    public class StoryMapper
    {
        public static StoryResponse MapToStoryResponse(Story story)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(story.Time).ToLocalTime();
            return new StoryResponse
            {
                PostedBy = story.PostedBy,
                CommentCount = story.CommentCount,
                Score = story.Score,
                Title = story.Title,
                Uri = story.Uri,
                Time = dateTime
            };
        }
    }
}
