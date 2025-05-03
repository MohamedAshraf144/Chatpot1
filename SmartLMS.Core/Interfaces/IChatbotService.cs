using SmartLMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLMS.Core.Interfaces
{
    public interface IChatbotService
    {
        Task<string> GetResponseAsync(string userMessage, IEnumerable<ChatMessage> conversationHistory);
    }
}