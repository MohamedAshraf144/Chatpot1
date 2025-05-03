using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartLMS.Core.Interfaces;
using SmartLMS.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace SmartLMS.Application.Services
{
    public class GoogleGeminiChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public GoogleGeminiChatbotService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"];
            _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={_apiKey}";
        }

        public async Task<string> GetResponseAsync(string userMessage, IEnumerable<ChatMessage> conversationHistory)
        {
            try
            {
                var messages = new List<object>();

                foreach (var msg in conversationHistory)
                {
                    messages.Add(new
                    {
                        role = msg.IsFromBot ? "model" : "user",
                        parts = new[]
                        {
                            new { text = msg.Content }
                        }
                    });
                }

                messages.Add(new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = userMessage }
                    }
                });

                var requestData = new
                {
                    contents = messages.ToArray(),
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"استجابة Google Gemini: {responseBody}");

                if (!response.IsSuccessStatusCode)
                {
                    return $"حدث خطأ في الاتصال بـ Google Gemini: {response.StatusCode}";
                }

                using var doc = JsonDocument.Parse(responseBody);

                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var content1) &&
                    content1.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var textElement))
                {
                    return textElement.GetString() ?? "لم يتم تلقي رد مناسب.";
                }

                return "عذرًا، لم أتمكن من فهم الرد من Google Gemini.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ: {ex.Message}");
                return "عذرًا، حدثت مشكلة أثناء معالجة طلبك. يرجى المحاولة مرة أخرى لاحقًا.";
            }
        }
    }
}