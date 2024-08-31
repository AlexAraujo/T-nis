using Nest;
using Microsoft.Extensions.Logging;
using Elasticsearch.Net;
using System.Text.Json;
using TnisSearchAPI.Models;

namespace TnisSearchAPI.Services
{
    public class ElasticsearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticsearchService> _logger;
        private readonly UnsplashService _unsplashService;

        public ElasticsearchService(IElasticClient elasticClient, ILogger<ElasticsearchService> logger, UnsplashService unsplashService)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _unsplashService = unsplashService;
        }

        public async Task<ISearchResponse<ImageModel>> Search(string descricao, string cor, string estilo)
        {
            _logger.LogInformation($"Searching with descricao: {descricao}, cor: {cor}, estilo: {estilo}");

            var searchResponse = await _elasticClient.SearchAsync<ImageModel>(s => s
                .Index("tnis_images")
                .Query(q => 
                {
                    if (string.IsNullOrWhiteSpace(descricao) && string.IsNullOrWhiteSpace(cor) && string.IsNullOrWhiteSpace(estilo))
                    {
                        return q.MatchAll();
                    }
                    else
                    {
                        return q.Bool(b => b
                            .Should(
                                s => s.Match(m => m.Field(f => f.Descricao).Query(descricao).Fuzziness(Fuzziness.Auto)),
                                s => s.Match(m => m.Field(f => f.Cor).Query(cor).Fuzziness(Fuzziness.Auto)),
                                s => s.Match(m => m.Field(f => f.Estilo).Query(estilo).Fuzziness(Fuzziness.Auto))
                            )
                            .MinimumShouldMatch(1)
                        );
                    }
                })
                .Size(50) // Aumentado para 50 resultados
            );

            _logger.LogInformation($"Search completed. Total hits: {searchResponse.Total}");
            _logger.LogInformation($"Query DSL: {_elasticClient.RequestResponseSerializer.SerializeToString(searchResponse.ApiCall.RequestBodyInBytes)}");
            _logger.LogInformation($"Response: {_elasticClient.RequestResponseSerializer.SerializeToString(searchResponse.ApiCall.ResponseBodyInBytes)}");

            if (!searchResponse.IsValid)
            {
                _logger.LogError($"Elasticsearch error: {searchResponse.ServerError?.Error}");
                _logger.LogError($"Debug information: {searchResponse.DebugInformation}");
                _logger.LogError($"Original exception: {searchResponse.OriginalException}");
            }
            else
            {
                foreach (var hit in searchResponse.Hits)
                {
                    _logger.LogInformation($"Hit: ID = {hit.Id}, Score = {hit.Score}, Source = {JsonSerializer.Serialize(hit.Source)}");
                }
            }

            return searchResponse;
        }

        public async Task<bool> CreateIndexIfNotExists()
        {
            if ((await _elasticClient.Indices.ExistsAsync("tnis_images")).Exists)
            {
                await _elasticClient.Indices.DeleteAsync("tnis_images");
            }

            var createIndexResponse = await _elasticClient.Indices.CreateAsync("tnis_images", c => c
                .Map<ImageModel>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Url))
                        .Text(t => t.Name(n => n.Descricao))
                        .Keyword(k => k.Name(n => n.Cor))
                        .Keyword(k => k.Name(n => n.Estilo))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                _logger.LogError($"Failed to create index: {createIndexResponse.DebugInformation}");
            }

            return createIndexResponse.IsValid;
        }

        public async Task<bool> InsertTestData(string query = "sneakers")
        {
            var sneakerImages = await _unsplashService.SearchSneakerImages(query);
            
            var bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index("tnis_images")
                .IndexMany(sneakerImages)
            );

            if (!bulkResponse.IsValid)
            {
                _logger.LogError($"Failed to insert test data: {bulkResponse.DebugInformation}");
            }
            else
            {
                _logger.LogInformation($"Successfully inserted {sneakerImages.Count} test images");
            }

            return bulkResponse.IsValid;
        }

        public async Task<long> GetDocumentCount()
        {
            var countResponse = await _elasticClient.CountAsync<ImageModel>(c => c
                .Index("tnis_images")
            );

            if (!countResponse.IsValid)
            {
                _logger.LogError($"Failed to get document count: {countResponse.DebugInformation}");
            }

            return countResponse.Count;
        }

        public async Task<List<ImageModel>> GetAllDocuments()
        {
            var searchResponse = await _elasticClient.SearchAsync<ImageModel>(s => s
                .Index("tnis_images")
                .Query(q => q.MatchAll())
                .Size(100)
            );

            if (!searchResponse.IsValid)
            {
                _logger.LogError($"Failed to get all documents: {searchResponse.DebugInformation}");
                return new List<ImageModel>();
            }

            return searchResponse.Documents.ToList();
        }
    }

    public class ImageDocument
    {
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}