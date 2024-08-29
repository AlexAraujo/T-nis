using System.Collections.Generic;
using TnisSearchApi.Interfaces;
using TnisSearchApi.Models;

namespace TnisSearchApi.Services
{
    public class SearchService : ISearchService
    {
        public SearchResult Search(SearchRequest request)
        {
            // Aqui você implementaria a lógica real de busca
            // Por enquanto, vamos retornar alguns dados de exemplo
            var descricao = $"{request.Descricao} - {request.Cor} - {request.Estilo}";
            var images = new List<ImageInfo>
            {
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
                new ImageInfo { Url = "https://dcdn.mitiendanube.com/stores/002/668/124/products/tnaf11-823fdf4604fc3db27316716430371832-1024-1024.jpg", Description = descricao },
            };

            return new SearchResult { Images = images };
        }
    }
}