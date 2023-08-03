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
        public async Task<IActionResult> GetBestStories(int n = 10)
        {
            if (n < 1) return BadRequest();

            var result = await _hackerNewsService.GetBestStories(n);

            if (result is null || !result.Any())
                return NoContent();

            return Ok(result);
        }
    }
}
