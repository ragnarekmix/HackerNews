using HackerNews.Core;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Api.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public HackerNewsController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("best")]
        public async Task<IActionResult> GetBestStories()
        {
            var result = await _hackerNewsService.GetBestStories();

            if (result is null || !result.Any())
                return NoContent();

            return Ok(result);
        }
    }
}
