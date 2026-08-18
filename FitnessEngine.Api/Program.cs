using Elastic.Clients.Elasticsearch;
using FitnessEngine.Api.Data;
using FitnessEngine.Api.Repositories;
using FitnessEngine.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Controllers
// --------------------------------------------------

builder.Services.AddControllers();

// --------------------------------------------------
// Swagger
// --------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------
// CORS
// --------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// --------------------------------------------------
// MySQL + Entity Framework Core
// --------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FitnessDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});

// --------------------------------------------------
// Elasticsearch
// --------------------------------------------------

var elasticsearchUri =
    builder.Configuration["Elasticsearch:Uri"]
    ?? "http://localhost:9200";

var elasticsearchClient =
    new ElasticsearchClient(
        new ElasticsearchClientSettings(
            new Uri(elasticsearchUri)
        )
    );

builder.Services.AddSingleton(elasticsearchClient);

// --------------------------------------------------
// HttpClient
// Used for Ollama AI requests
// --------------------------------------------------

builder.Services.AddHttpClient();

// --------------------------------------------------
// Repositories
// --------------------------------------------------

builder.Services.AddScoped<WorkoutRepository>();

// --------------------------------------------------
// Services
// --------------------------------------------------

builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<ElasticsearchService>();

// --------------------------------------------------
// Static files / frontend
// --------------------------------------------------

builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// --------------------------------------------------
// Swagger
// --------------------------------------------------

app.UseSwagger();
app.UseSwaggerUI();

// --------------------------------------------------
// Middleware
// --------------------------------------------------

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthorization();

// --------------------------------------------------
// API Controllers
// --------------------------------------------------

app.MapControllers();

// --------------------------------------------------
// Frontend
// --------------------------------------------------

app.MapFallbackToFile("index.html");

app.Run();
