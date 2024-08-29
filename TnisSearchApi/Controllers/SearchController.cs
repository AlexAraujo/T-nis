using Microsoft.AspNetCore.Mvc;
using TnisSearchApi.Interfaces;
using TnisSearchApi.Models;

namespace TnisSearchApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost]
        public ActionResult<SearchResult> Search([FromBody] SearchRequest request)
        {
            var result = _searchService.Search(request);
            return Ok(result);
        }
    }
}