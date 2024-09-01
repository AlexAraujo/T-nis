using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;

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
    public async Task<IActionResult> SearchSneakerImages([FromBody] SearchRequest request)
    {
        var images = await _unsplashService.SearchSneakerImages(request.Descricao, request.Cor, request.Estilo);
        return Ok(new { images });
    }
}

public class SearchRequest
{
    public string Descricao { get; set; }
    public string Cor { get; set; }
    public string Estilo { get; set; }
}