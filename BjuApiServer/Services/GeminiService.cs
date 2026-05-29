using System.Text;
using System.Text.Json;

namespace BjuApiServer.Services;

public class GeminiResponse { public Candidate[]? Candidates { get; set; } }
public class Candidate { public Content? Content { get; set; } }
public class Content { public Part[]? Parts { get; set; } }
public class Part { public string? Text { get; set; } }

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiService> _logger;
    private readonly string _geminiApiUrl;
    private readonly string _modelName;

    public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Gemini API key is not configured in appsettings.");

        // Дефолт: доступна для твого ключа модель
        _modelName = configuration["Gemini:Model"] ?? "gemini-2.5-flash";
        _geminiApiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelName}:generateContent?key={apiKey}";
    }

    public async Task<string> GenerateMealPlanAsync(string prompt)
    {
        _logger.LogInformation("Sending request to Gemini API using model {Model}", _modelName);

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new
            {
                temperature = 0.5,
                responseMimeType = "application/json"
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_geminiApiUrl, jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Gemini API error. Status: {Status} Body: {Body}", response.StatusCode, errorBody);
            return $"AI Error: {response.StatusCode} - {errorBody}";
        }

        var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        var resultText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrEmpty(resultText))
        {
            _logger.LogWarning("Gemini API returned empty response.");
            return "AI service returned an empty response.";
        }

        return resultText;
    }

    public async Task<string> AnalyzeFoodImageAsync(string base64Image)
    {
        _logger.LogInformation("Sending image to Gemini API using model {Model}", _modelName);

        var prompt = "Analyze this food image. Identify what it is, and provide a reasonable estimation of its nutritional values per 100g. Return ONLY a valid JSON object matching exactly this structure with no markdown formatting: {\"Name\": \"Food name in Ukrainian\", \"CaloriesPer100g\": 250, \"ProteinPer100g\": 10.5, \"FatPer100g\": 8.2, \"CarbsPer100g\": 30.1}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = prompt },
                        new
                        {
                            inlineData = new
                            {
                                mimeType = "image/jpeg",
                                data = base64Image
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.4,
                responseMimeType = "application/json"
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_geminiApiUrl, jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Gemini API error during image analysis. Status: {Status} Body: {Body}", response.StatusCode, errorBody);
            throw new Exception($"Gemini API error: {response.StatusCode}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var resultText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrEmpty(resultText))
        {
            _logger.LogWarning("Gemini API returned empty response for image analysis.");
            throw new Exception("AI service returned an empty response.");
        }

        return resultText;
    }
}