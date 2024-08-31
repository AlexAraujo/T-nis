using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TnisSearchAPI.Models;
using Microsoft.Extensions.Logging; // Added for logging

namespace TnisSearchAPI.Services
{
    public class UnsplashService
    {
        private readonly HttpClient _httpClient;
        private readonly string _applicationId;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private const int MaxPerPage = 30;
        private const int MaxPages = 5; // Limite para 5 páginas (150 imagens no total)
        private readonly ILogger<UnsplashService> _logger; // Added for logging

        public string ApplicationId => _applicationId;
        public string AccessKey => _accessKey;
        public bool HasSecretKey => !string.IsNullOrEmpty(_secretKey);

        public UnsplashService(HttpClient httpClient, IConfiguration configuration, ILogger<UnsplashService> logger)
        {
            _httpClient = httpClient;
            _applicationId = configuration["Unsplash:ApplicationId"];
            _accessKey = configuration["Unsplash:AccessKey"];
            _secretKey = configuration["Unsplash:SecretKey"];
            _logger = logger;
            
            _logger.LogInformation($"Unsplash Application ID: {_applicationId}");
            _logger.LogInformation($"Unsplash Access Key: {_accessKey}");
            _logger.LogInformation($"Unsplash Secret Key: {_secretKey.Substring(0, 4)}..."); // Log only the first 4 characters of the secret key

            if (string.IsNullOrEmpty(_accessKey))
            {
                _logger.LogError("Unsplash Access Key is not configured.");
                throw new InvalidOperationException("Unsplash Access Key is not configured.");
            }
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_accessKey}");
        }

        public async Task<List<ImageModel>> SearchSneakerImages(string query, string color = "", string style = "")
        {
            try
            {
                query = string.IsNullOrEmpty(query) ? "sneakers" : query;
                var searchQuery = $"{query} {color} {style}".Trim();
                _logger.LogInformation($"Searching Unsplash with query: {searchQuery}");

                var allImages = new List<ImageModel>();

                for (int page = 1; page <= MaxPages; page++)
                {
                    var url = $"search/photos?query={searchQuery}&per_page={MaxPerPage}&page={page}";
                    _logger.LogInformation($"Requesting URL: {url}");

                    var response = await _httpClient.GetFromJsonAsync<UnsplashSearchResponse>(url);

                    if (response?.Results == null || response.Results.Count == 0)
                    {
                        _logger.LogInformation($"No results found for page {page}");
                        break;
                    }

                    _logger.LogInformation($"Found {response.Results.Count} images on page {page}");

                    allImages.AddRange(response.Results.Select(r => new ImageModel
                    {
                        Url = r.Urls.Regular,
                        Descricao = r.Description ?? r.AltDescription ?? $"Imagem de {query}",
                        Cor = color,
                        Estilo = style
                    }));

                    if (response.TotalPages <= page)
                    {
                        _logger.LogInformation($"Reached last page: {page}");
                        break;
                    }
                }

                _logger.LogInformation($"Total images found: {allImages.Count}");
                return allImages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnsplashService.SearchSneakerImages");
                throw;
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