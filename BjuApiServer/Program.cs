using BjuApiServer.Data;
using BjuApiServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=bju.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// Services
builder.Services.AddScoped<BjuCalculationService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));

// CORS (дозволити все, можна звузити за потреби)
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod()));

// MVC / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Увімкнути Swagger і в продакшені
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();