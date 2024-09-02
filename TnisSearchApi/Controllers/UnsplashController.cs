using Microsoft.AspNetCore.Mvc;
using TnisSearchAPI.Services;
using TnisSearchAPI.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace TnisSearchAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnsplashController : ControllerBase
    {
        private readonly UnsplashService _unsplashService;

        public UnsplashController(UnsplashService unsplashService)
        {
            _unsplashService = unsplashService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request payload.");
            }

            try
            {
                var images = await _unsplashService.SearchSneakerImages(request.Descricao, request.Cor, request.Estilo);
                return Ok(new { Images = images });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)(ex.StatusCode ?? System.Net.HttpStatusCode.InternalServerError), $"Error calling Unsplash API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
