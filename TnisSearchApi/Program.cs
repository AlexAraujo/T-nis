using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TnisSearchApi.Interfaces;
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

// Replace the existing UnsplashService registration with this:
builder.Services.AddHttpClient<UnsplashService>(client =>
{
    client.BaseAddress = new Uri("https://api.unsplash.com/");
});

// Remove these lines:
// builder.Services.AddHttpClient<ChatGptService>();
// builder.Services.AddSingleton<ChatGptService>();

var app = builder.Build();

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

// After the UnsplashService configuration
var unsplashConfig = builder.Configuration.GetSection("Unsplash");