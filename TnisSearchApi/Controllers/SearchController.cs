using Microsoft.AspNetCore.Mvc;
using TnisSearchAPI.Services;
using TnisSearchAPI.Models;

namespace TnisSearchAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ElasticsearchService _elasticsearchService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ElasticsearchService elasticsearchService, ILogger<SearchController> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string descricao, [FromQuery] string cor, [FromQuery] string estilo)
        {
            var searchResponse = await _elasticsearchService.Search(descricao, cor, estilo);

            var results = searchResponse.Hits.Select(hit => new ImageModel
            {
                Url = hit.Source.Url,
                Descricao = hit.Source.Descricao,
                Cor = hit.Source.Cor,
                Estilo = hit.Source.Estilo
            }).ToList();

            return Ok(new
            {
                TotalHits = searchResponse.Total,
                Images = results,
                ElapsedMilliseconds = searchResponse.Took,
                IsValid = searchResponse.IsValid,
                ServerError = searchResponse.ServerError?.Error,
                DebugInfo = searchResponse.DebugInformation
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SearchRequest request)
        {
            var searchResponse = await _elasticsearchService.Search(request.Descricao, request.Cor, request.Estilo);

            var results = searchResponse.Hits.Select(hit => new ImageModel
            {
                Url = hit.Source.Url,
                Descricao = hit.Source.Descricao,
                Cor = hit.Source.Cor,
                Estilo = hit.Source.Estilo
            }).ToList();

            return Ok(new
            {
                TotalHits = searchResponse.Total,
                Images = results,
                ElapsedMilliseconds = searchResponse.Took,
                IsValid = searchResponse.IsValid,
                ServerError = searchResponse.ServerError?.Error,
                DebugInfo = searchResponse.DebugInformation
            });
        }
    }

    public class SearchRequest
    {
        public string Descricao { get; set; }
        public string Cor { get; set; }
        public string Estilo { get; set; }
    }
}