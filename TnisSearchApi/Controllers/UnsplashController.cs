using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TnisSearchAPI.Services;
using TnisSearchAPI.Models;
using System.Text.Json;
using System.Net.Http;

namespace TnisSearchAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnsplashController : ControllerBase
    {
        private readonly UnsplashService _unsplashService;
        private readonly ILogger<UnsplashController> _logger;

        public UnsplashController(UnsplashService unsplashService, ILogger<UnsplashController> logger)
        {
            _unsplashService = unsplashService;
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            try
            {
                _logger.LogInformation($"Received search request: {JsonSerializer.Serialize(request)}");
                _logger.LogInformation($"Searching for images with query: {request.Descricao}, color: {request.Cor}, style: {request.Estilo}");
                var images = await _unsplashService.SearchSneakerImages(request.Descricao, request.Cor, request.Estilo);
                _logger.LogInformation($"Found {images.Count} images");
                return Ok(new { Images = images });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error in UnsplashController.Search. StatusCode: {ex.StatusCode}");
                return StatusCode((int)ex.StatusCode, $"Error calling Unsplash API: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnsplashController.Search");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGet([FromQuery] string descricao = "", [FromQuery] string cor = "", [FromQuery] string estilo = "")
        {
            try
            {
                _logger.LogInformation($"Received GET search request: descricao={descricao}, cor={cor}, estilo={estilo}");
                var images = await _unsplashService.SearchSneakerImages(descricao, cor, estilo);
                _logger.LogInformation($"Found {images.Count} images");
                return Ok(new { Images = images });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error in UnsplashController.SearchGet. StatusCode: {ex.StatusCode}");
                return StatusCode((int)ex.StatusCode, $"Error calling Unsplash API: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnsplashController.SearchGet");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new
            {
                ApplicationId = _unsplashService.ApplicationId,
                AccessKey = _unsplashService.AccessKey,
                SecretKeyConfigured = _unsplashService.HasSecretKey
            });
        }
    }
}