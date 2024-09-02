using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace TnisSearchAPI.Services
{
    public class ChatGptService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ChatGptService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"];
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API Key is not configured or is invalid.");
            }
        }

        public async Task<string> SummarizeDescription(string text)
        {
            var prompt = $"Resuma isso em 4 palavras: {text}";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "system", content = "Você é um assistente de resumo de textos." },
            new { role = "user", content = prompt }
        },
                max_tokens = 50,  // Mantém o limite de tokens pequeno para obter respostas curtas
                temperature = 0.7  // Ajuste a "criatividade" do modelo
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseContent);

                // Acessa a string resumida retornada pela API
                var summarizedText = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return text;
            }
            else
            {
                // Divida o texto em palavras usando o espaço como delimitador
                var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Pegue as primeiras 4 palavras (ou menos, se o texto tiver menos de 4 palavras)
                var firstFourWords = words.Take(4);

                // Junte essas palavras em uma string, separadas por espaço
                return string.Join(' ', firstFourWords);
            }
        }

        public async Task<string> ConverterHexadecimalEmNomeDaCor(string hexadecimal)
        {
            var prompt = $"Me fale o nome da cor mais proxima desse hexadecimal: {hexadecimal}";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Você é um assistente de descoberta de cores em hexadecimal." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 50,  // Mantém o limite de tokens pequeno para obter respostas curtas
                temperature = 0.7  // Ajuste a "criatividade" do modelo
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(responseContent);

                    // Acessa a string resumida retornada pela API
                    var summarizedText = jsonDoc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return summarizedText;
                }
                else
                {
                    return "Todas";
                }
            }
            catch (Exception ex)
            {
                return $"Ocorreu um erro: {ex.Message}";
            }
        }

    }

    public class ChatGptResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}