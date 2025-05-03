using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Interfaces;

namespace SmartLMS.Application.Services
{
    public class MockChatbotService : IChatbotService
    {
        public Task<string> GetResponseAsync(string userMessage, IEnumerable<ChatMessage> conversationHistory)
        {
            // محاكاة تأخير استجابة الخادم
            Task.Delay(300).Wait();

            string response = $"هذا رد اختباري على رسالتك: \"{userMessage}\". في النسخة النهائية، سيتم استخدام Claude AI للحصول على إجابات أكثر ذكاءً.";

            return Task.FromResult(response);
        }
    }
}