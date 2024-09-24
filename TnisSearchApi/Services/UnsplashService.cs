using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TnisSearchAPI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace TnisSearchAPI.Services
{
    public class UnsplashService
    {
        private readonly HttpClient _httpClient;
        private readonly ChatGptService _chatGptService;
        private readonly string _applicationId;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private const int MaxPerPage = 30;
        private const int MaxPages = 5;
        private readonly ILogger<UnsplashService> _logger;

        public string ApplicationId => _applicationId;
        public string AccessKey => _accessKey;
        public bool HasSecretKey => !string.IsNullOrEmpty(_secretKey);


        public UnsplashService(
            HttpClient httpClient,
            IConfiguration configuration,
            ChatGptService chatGptService,
            ILogger<UnsplashService> logger)
        {
            _httpClient = httpClient;
            _applicationId = configuration["Unsplash:ApplicationId"];
            _accessKey = configuration["Unsplash:AccessKey"];
            _secretKey = configuration["Unsplash:SecretKey"];
            _logger = logger;
            _chatGptService = chatGptService;

            if (string.IsNullOrEmpty(_accessKey))
                throw new InvalidOperationException("Unsplash Access Key is not configured.");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_accessKey}");
        }

        public async Task<List<ImageModel>> SearchSneakerImages(string query, string color = "", string style = "")
        {
            if (string.IsNullOrEmpty(query))
            {
                query = "cat";
            }

            try
            {
                color = await _chatGptService.ConverterHexadecimalEmNomeDaCor(color);
                var searchQuery = $"animal {query}".Trim();

                var allImages = new List<ImageModel>();

                for (int page = 1; page <= MaxPages; page++)
                {
                    var url = $"search/photos?query={searchQuery}&per_page={MaxPerPage}&page={page}";

                    var response = await _httpClient.GetFromJsonAsync<UnsplashSearchResponse>(url);

                    if (response?.Results == null || response.Results.Count == 0)
                    {
                        break;
                    }

                    var tasks = response.Results.Select(async r =>
                    {
                        var description = r.Description ?? "Descrição não disponível";

                        return new ImageModel
                        {
                            Url = r.Urls?.Regular ?? "URL não disponível",
                            Descricao = await _chatGptService.SummarizeDescription(description),
                            Cor = color,
                            Estilo = style
                        };
                    });

                    allImages.AddRange(await Task.WhenAll(tasks));

                    if (response.TotalPages <= page)
                    {
                        break;
                    }
                }

                return allImages;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao chamar a API do Unsplash.");
                throw new InvalidOperationException("Erro ao chamar a API do Unsplash.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                throw new InvalidOperationException("Erro inesperado.", ex);
            }
        }
    }

    public class UnsplashSearchResponse
    {
        public List<UnsplashImage> Results { get; set; }
        public int TotalPages { get; set; }
    }

    public class UnsplashImage
    {
        public UnsplashImageUrls Urls { get; set; }
        public string Description { get; set; }
        public string AltDescription { get; set; }
    }

    public class UnsplashImageUrls
    {
        public string Regular { get; set; }
    }
}