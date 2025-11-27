using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace BjuApiServer.Services
{
    public class OllamaOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }

    public class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
        public bool Done { get; set; }
    }

    public class OllamaService
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaOptions _options;
        private readonly ILogger<OllamaService> _logger;

        public OllamaService(HttpClient httpClient, IOptions<OllamaOptions> options, ILogger<OllamaService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;

            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                throw new InvalidOperationException("Ollama BaseUrl is not configured in appsettings.json.");
            }
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }

        
        public async Task<string> GenerateMealPlanAsync(string userPrompt)
        {
            try
            {
                var requestPayload = new { model = _options.Model, prompt = userPrompt, stream = false };
                var jsonPayload = JsonSerializer.Serialize(requestPayload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending non-stream request to Ollama API.");
                var httpResponse = await _httpClient.PostAsync("/api/generate", content);
                httpResponse.EnsureSuccessStatusCode();

                var ollamaResponse = await httpResponse.Content.ReadFromJsonAsync<OllamaResponse>();
                return ollamaResponse?.Response ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in non-streaming Ollama request.");
                throw;
            }
        }

       
        public async IAsyncEnumerable<string> GenerateMealPlanStreamAsync(
            string userPrompt,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await GenerateMealPlanAsync(userPrompt);

            if (!cancellationToken.IsCancellationRequested)
            {
                yield return response;
            }
        }
    }
}