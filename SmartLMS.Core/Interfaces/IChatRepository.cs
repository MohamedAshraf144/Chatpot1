using System.Collections.Generic;
using System.Threading.Tasks;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Entities.Chat;

namespace SmartLMS.Core.Interfaces
{
    public interface IChatRepository
    {
        // Chat Session Methods - لنظام المحادثة مع Chatbot
        Task<ChatSession> CreateSessionAsync(string userId, string title);
        Task<ChatSession> GetSessionByIdAsync(int sessionId);
        Task<IEnumerable<ChatSession>> GetUserSessionsAsync(string userId);
        Task<IEnumerable<ChatMessage>> GetSessionMessagesAsync(int sessionId);
        Task<ChatMessage> SaveMessageAsync(ChatMessage message);
        Task<bool> UpdateSessionLastActivityAsync(int sessionId);

        // Conversation Methods - لنظام المحادثة بين المستخدمين
        Task<Conversation> CreateConversationAsync(Conversation conversation);
        Task<Conversation> GetConversationByIdAsync(int conversationId, bool includeParticipants = false);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
        Task<bool> UpdateConversationLastActivityAsync(int conversationId);

        // Message Methods
        Task<Message> AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId, int pageNumber = 1, int pageSize = 20);
        Task<int> GetConversationMessagesCountAsync(int conversationId);
        Task<int> GetUnreadMessagesCountAsync(int conversationId, string userId);
        Task<bool> MarkMessagesAsReadAsync(int conversationId, string userId);
        Task<bool> AddMessageReadReceiptAsync(MessageReadReceipt readReceipt);

        // Participant Methods
        Task<bool> AddParticipantAsync(ConversationParticipant participant);
        Task<bool> RemoveParticipantAsync(int conversationId, string userId);
        Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(int conversationId);
        Task<ConversationParticipant> GetParticipantAsync(int conversationId, string userId);
        Task<IEnumerable<ConversationParticipant>> GetConversationAdminsAsync(int conversationId);
        Task<bool> IsUserInConversationAsync(int conversationId, string userId);
    }
}