using System.Net.Http.Headers;
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

    private const string ModelName = "gemini-1.5-flash-latest"; 
    
    public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;

       
        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured in appsettings.json");
        }

        
        _geminiApiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent?key={apiKey}";
    }

    public async Task<string> GenerateMealPlanAsync(string prompt)
    {
        _logger.LogInformation("Sending request to Gemini API.");

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_geminiApiUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API returned an error. Status: {StatusCode}, Body: {ErrorBody}", response.StatusCode, errorBody);
                return $"Error from AI service: {response.ReasonPhrase}";
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            var resultText = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (string.IsNullOrEmpty(resultText))
            {
                _logger.LogWarning("Gemini API returned an empty or invalid response.");
                return "AI service returned an empty response.";
            }

            return resultText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while calling Gemini API.");
            throw;
        }
    }
}
