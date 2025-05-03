using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartLMS.Application.DTOs;
using SmartLMS.Application.DTOs.Chat;

namespace SmartLMS.Core.Interfaces
{
    public interface IChatService
    {
        // خدمات الشات بوت
        Task<IEnumerable<ChatSessionDto>> GetUserSessionsAsync(string userId);
        Task<IEnumerable<ChatMessageDto>> GetSessionMessagesAsync(int sessionId);
        Task<ChatResponseDto> ProcessMessageAsync(ChatRequestDto request, string userId);

        // خدمات المحادثات بين المستخدمين
        Task<ConversationDto> CreateConversationAsync(string userId, List<string> participantIds, string title = null);
        Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId);
        Task<ConversationDto> GetConversationByIdAsync(int conversationId, string userId);
        Task<MessagesResponseDto> GetConversationMessagesAsync(int conversationId, string userId, int page = 1, int pageSize = 20);
        Task<MessageDto> SendMessageAsync(int conversationId, string senderId, string content, string attachment = null);
        Task<bool> MarkConversationAsReadAsync(int conversationId, string userId);
        Task<bool> AddParticipantToConversationAsync(int conversationId, string userId, string participantId);
        Task<bool> RemoveParticipantFromConversationAsync(int conversationId, string userId, string participantToRemoveId);
    }
}