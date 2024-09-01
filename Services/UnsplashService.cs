using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using System.Text.Json;

public class UnsplashService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UnsplashService> _logger;
    private readonly OpenAIAPI _openAiApi;
    private const int MaxPages = 3;
    private const int MaxPerPage = 30;

    public UnsplashService(HttpClient httpClient, ILogger<UnsplashService> logger, OpenAIAPI openAiApi)
    {
        _httpClient = httpClient;
        _logger = logger;
        _openAiApi = openAiApi;
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID YOUR_UNSPLASH_ACCESS_KEY");
    }

    private async Task<string> SummarizeDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return "Descrição indisponível";
        }

        try
        {
            var completionRequest = new CompletionRequest
            {
                Model = "text-davinci-003",
                Prompt = $"Resuma isso em 4 palavras: \"{description}\"",
                MaxTokens = 10
            };

            var completionResult = await _openAiApi.Completions.CreateCompletionAsync(completionRequest);
            return completionResult.Completions[0].Text.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error summarizing description with OpenAI");
            return string.Join(" ", description.Split(' ').Take(4)); // Fallback to simple 4-word summary
        }
    }

    public async Task<List<ImageModel>> SearchSneakerImages(string query, string color = "", string style = "")
    {
        try
        {
            var allImages = new List<ImageModel>();
            var searchQuery = $"{query} {color} {style}".Trim();

            for (int page = 1; page <= MaxPages; page++)
            {
                var url = $"https://api.unsplash.com/search/photos?query={searchQuery}&per_page={MaxPerPage}&page={page}";
                var response = await _httpClient.GetFromJsonAsync<UnsplashSearchResponse>(url);

                if (response?.Results == null || response.Results.Count == 0)
                {
                    break;
                }

                var imageTasks = response.Results.Select(async r => new ImageModel
                {
                    Url = r.Urls.Regular,
                    Descricao = await SummarizeDescription(r.Description ?? r.AltDescription ?? $"Imagem de {query}"),
                    Cor = color,
                    Estilo = style
                });

                allImages.AddRange(await Task.WhenAll(imageTasks));

                if (response.TotalPages <= page)
                {
                    break;
                }
            }

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
    public UnsplashUrls Urls { get; set; }
    public string Description { get; set; }
    public string AltDescription { get; set; }
}

public class UnsplashUrls
{
    public string Regular { get; set; }
}

public class ImageModel
{
    public string Url { get; set; }
    public string Descricao { get; set; }
    public string Cor { get; set; }
    public string Estilo { get; set; }
}