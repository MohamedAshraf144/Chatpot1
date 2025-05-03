using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Interfaces;

namespace SmartLMS.Application.Services
{
    public class ClaudeChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _modelName;

        public ClaudeChatbotService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Claude:ApiKey"];
            _apiUrl = configuration["Claude:ApiUrl"] ?? "https://api.anthropic.com/v1/messages";
            _modelName = configuration["Claude:ModelName"] ?? "claude-3-sonnet-20240229";

            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        }

        public async Task<string> GetResponseAsync(string userMessage, IEnumerable<ChatMessage> conversationHistory)
        {
            try
            {
                Console.WriteLine($"Claude URL: {_apiUrl}");
                Console.WriteLine($"Claude Model: {_modelName}");

                // تسجيل المحادثة السابقة للتشخيص
                Console.WriteLine("محادثة سابقة:");
                foreach (var msg in conversationHistory)
                {
                    Console.WriteLine($"{(msg.IsFromBot ? "Assistant" : "User")}: {msg.Content}");
                }

                var messages = new List<object>();

                foreach (var msg in conversationHistory)
                {
                    messages.Add(new
                    {
                        role = msg.IsFromBot ? "assistant" : "user",
                        content = msg.Content
                    });
                }

                messages.Add(new
                {
                    role = "user",
                    content = userMessage
                });

                var requestData = new
                {
                    model = _modelName,
                    messages = messages,
                    max_tokens = 1000,
                    system = "أنت مساعد تعليمي في نظام إدارة تعلم (LMS). مهمتك مساعدة الطلاب والمعلمين في المواضيع التعليمية بإجابات دقيقة ومختصرة وودية."
                };

                // تسجيل بيانات الطلب للتشخيص
                Console.WriteLine("بيانات الطلب:");
                Console.WriteLine(JsonSerializer.Serialize(requestData, new JsonSerializerOptions { WriteIndented = true }));

                var content = new StringContent(
                    JsonSerializer.Serialize(requestData),
                    Encoding.UTF8,
                    "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("استجابة Claude الخام:");
                Console.WriteLine(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"خطأ في استجابة Claude: {response.StatusCode}");
                    Console.WriteLine(responseBody);
                    return $"حدث خطأ في الاتصال بـ Claude: {response.StatusCode} - {responseBody}";
                }

                using var doc = JsonDocument.Parse(responseBody);

                // استخراج النص من content array
                if (doc.RootElement.TryGetProperty("content", out var contentArray) &&
                    contentArray.ValueKind == JsonValueKind.Array && contentArray.GetArrayLength() > 0)
                {
                    var firstItem = contentArray[0];
                    if (firstItem.TryGetProperty("text", out var textElement))
                    {
                        return textElement.GetString() ?? "لم يتم تلقي رد من Claude.";
                    }
                    else
                    {
                        return "Claude أرجع رد غير متوقع (لا يحتوي على text).";
                    }
                }
                else
                {
                    return "Claude أرجع رد فارغ أو غير صالح.";
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return $"حدث خطأ في الاتصال بـ Claude: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error calling Claude API: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return "عذرًا، حدثت مشكلة أثناء معالجة طلبك. يرجى المحاولة مرة أخرى لاحقًا.";
            }
        }
    }
}
