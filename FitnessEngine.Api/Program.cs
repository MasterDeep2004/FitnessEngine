using FitnessEngine.Api.Repositories;
using FitnessEngine.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Controllers & Swagger
// -------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------------
// Enable CORS
// -------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// -------------------------
// HttpClient for LLM
// -------------------------
builder.Services.AddHttpClient();

// -------------------------
// DbContext (if using workouts from DB)
// -------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FitnessDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// -------------------------
// Repositories
// -------------------------
builder.Services.AddScoped<WorkoutRepository>();

// -------------------------
// Services
// -------------------------
builder.Services.AddScoped<RecommendationService>();

var app = builder.Build();

// -------------------------
// Middleware
// -------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
