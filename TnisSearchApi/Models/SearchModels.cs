using System.Collections.Generic;

namespace TnisSearchApi.Models
{
    public class SearchRequest
    {
        public string Cor { get; set; }
        public string Descricao { get; set; }
        public string Estilo { get; set; }
    }
    public class SearchResult
    {
        public List<ImageInfo> Images { get; set; }
    }

    public class ImageInfo
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }
}