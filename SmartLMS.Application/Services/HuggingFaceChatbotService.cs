using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SmartLMS.Core.Interfaces;
using SmartLMS.Core.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SmartLMS.Application.Services
{
    public class HuggingFaceChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;
        private readonly string _modelId;

        public HuggingFaceChatbotService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiToken = configuration["HuggingFace:ApiToken"];
            _modelId = configuration["HuggingFace:ModelId"] ?? "mistralai/Mistral-7B-Instruct-v0.3";

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiToken}");
        }

        public async Task<string> GetResponseAsync(string userMessage, IEnumerable<ChatMessage> conversationHistory)
        {
            try
            {
                // بناء المحادثة بالتنسيق المناسب
                string prompt = BuildPrompt(userMessage, conversationHistory);

                var requestData = new
                {
                    inputs = prompt,
                    parameters = new
                    {
                        max_new_tokens = 500,
                        temperature = 0.7,
                        top_p = 0.95,
                        do_sample = true
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    $"https://api-inference.huggingface.co/models/{_modelId}",
                    content);

                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"خطأ من Hugging Face: {responseBody}");
                    return $"عذرًا، حدثت مشكلة في معالجة طلبك: {response.StatusCode}";
                }

                // معالجة الاستجابة حسب نوع النموذج
                return ExtractResponseFromHuggingFace(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ: {ex.Message}");
                return "عذرًا، حدثت مشكلة أثناء معالجة طلبك. يرجى المحاولة مرة أخرى لاحقًا.";
            }
        }

        private string BuildPrompt(string userMessage, IEnumerable<ChatMessage> history)
        {
            // ضبط التنسيق حسب النموذج المستخدم (مثال لـ Mistral)
            StringBuilder promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("<s>");

            foreach (var message in history)
            {
                string role = message.IsFromBot ? "assistant" : "user";
                promptBuilder.AppendLine($"[{role}]: {message.Content}");
            }

            promptBuilder.AppendLine($"[user]: {userMessage}");
            promptBuilder.AppendLine("[assistant]:");

            return promptBuilder.ToString();
        }

        private string ExtractResponseFromHuggingFace(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);

                // للنماذج التي ترجع نص مباشرة
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var firstElement = doc.RootElement[0];
                    if (firstElement.TryGetProperty("generated_text", out var textElement))
                    {
                        string fullText = textElement.GetString();

                        // استخراج الجزء المناسب من النص الناتج
                        int assistantIndex = fullText.LastIndexOf("[assistant]:");
                        if (assistantIndex >= 0)
                        {
                            string assistantResponse = fullText.Substring(assistantIndex + 12).Trim();
                            // اقتطاع أي استمرار للمحادثة
                            int nextUserIndex = assistantResponse.IndexOf("[user]:");
                            if (nextUserIndex > 0)
                            {
                                assistantResponse = assistantResponse.Substring(0, nextUserIndex).Trim();
                            }
                            return assistantResponse;
                        }
                        return fullText.Trim();
                    }
                }

                // للحالات الأخرى
                return "عذرًا، لم أستطع فهم الرد من النموذج.";
            }
            catch
            {
                return "عذرًا، حدثت مشكلة في معالجة الرد.";
            }
        }
    }
}