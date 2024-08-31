using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnisSearchApi.Interfaces;
using TnisSearchApi.Services;
using Nest;
using TnisSearchAPI.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

// Add this line at the beginning of the file, before var builder = WebApplication.CreateBuilder(args);
// Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add this line to use User Secrets in Development environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Substitua a configuração CORS existente por esta:
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Ajuste para a URL do seu frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Registre o serviço
builder.Services.AddScoped<ISearchService, SearchService>();

// Configuração do Elasticsearch
var elasticsearchSettings = builder.Configuration.GetSection("Elasticsearch");
var url = elasticsearchSettings["Url"];

var settings = new ConnectionSettings(new Uri(url))
    .DefaultIndex("tnis_images");

var client = new ElasticClient(settings);

builder.Services.AddSingleton<IElasticClient>(client);

// Registre o serviço de Elasticsearch
builder.Services.AddSingleton<ElasticsearchService>();

// Replace the existing UnsplashService registration with this:
builder.Services.AddHttpClient<UnsplashService>(client =>
{
    client.BaseAddress = new Uri("https://api.unsplash.com/");
});

// Remove these lines:
// builder.Services.AddHttpClient<ChatGptService>();
// builder.Services.AddSingleton<ChatGptService>();

var app = builder.Build();

// Adicione esta linha após a criação do app
using (var scope = app.Services.CreateScope())
{
    var elasticsearchService = scope.ServiceProvider.GetRequiredService<ElasticsearchService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Attempting to create Elasticsearch index and insert test data...");
        var indexCreated = await elasticsearchService.CreateIndexIfNotExists();
        if (indexCreated)
        {
            logger.LogInformation("Elasticsearch index created successfully.");
            var dataInserted = await elasticsearchService.InsertTestData();
            if (dataInserted)
            {
                logger.LogInformation("Test data inserted successfully.");
                var documentCount = await elasticsearchService.GetDocumentCount();
                logger.LogInformation($"Current document count in index: {documentCount}");
                
                var allDocuments = await elasticsearchService.GetAllDocuments();
                logger.LogInformation($"All documents in index: {JsonSerializer.Serialize(allDocuments)}");
            }
            else
            {
                logger.LogWarning("Failed to insert test data.");
            }
        }
        else
        {
            logger.LogWarning("Failed to create Elasticsearch index or index already exists.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up Elasticsearch.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Após a configuração do UnsplashService
var unsplashAccessKey = builder.Configuration["Unsplash:AccessKey"];
Console.WriteLine($"Unsplash Access Key: {unsplashAccessKey}");

// After the UnsplashService configuration
var unsplashConfig = builder.Configuration.GetSection("Unsplash");
Console.WriteLine($"Unsplash Application ID: {unsplashConfig["ApplicationId"]}");
Console.WriteLine($"Unsplash Access Key: {unsplashConfig["AccessKey"]}");
Console.WriteLine($"Unsplash Secret Key: {unsplashConfig["SecretKey"]?.Substring(0, 4)}..."); // Log only the first 4 characters of the secret key