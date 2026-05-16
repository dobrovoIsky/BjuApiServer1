using BjuApiServer.Data;
using BjuApiServer.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext - PostgreSQL (Neon)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Services
builder.Services.AddScoped<BjuCalculationService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
builder.Services.AddSingleton<JsonDbService>();
builder.Services.AddHttpClient();

// CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod()));

// MVC / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Forward headers from Render reverse proxy (needed for Google OAuth redirect URI)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Auto-migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    await ProductSeeder.SeedProductsAsync(scope.ServiceProvider);
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Render handles HTTPS via reverse proxy - no UseHttpsRedirection needed
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
