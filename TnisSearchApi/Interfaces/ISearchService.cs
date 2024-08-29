using TnisSearchApi.Models;

namespace TnisSearchApi.Interfaces
{
    public interface ISearchService
    {
        SearchResult Search(SearchRequest request);
    }
}