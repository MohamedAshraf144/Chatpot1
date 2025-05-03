using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Entities.Chat;
using SmartLMS.Core.Enums; // إضافة مرجع لمساحة الاسم الخاصة بـ MessageType
using SmartLMS.Core.Interfaces;
using SmartLMS.Infrastructure.Data;

namespace SmartLMS.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------- طرق تنفيذ Chatbot ------------------
        public async Task<ChatSession> CreateSessionAsync(string userId, string title)
        {
            var session = new ChatSession
            {
                UserId = userId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };
            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<ChatSession> GetSessionByIdAsync(int sessionId)
        {
            return await _context.ChatSessions.FindAsync(sessionId);
        }

        public async Task<IEnumerable<ChatSession>> GetUserSessionsAsync(string userId)
        {
            return await _context.ChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.LastActivityAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetSessionMessagesAsync(int sessionId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatSessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<bool> UpdateSessionLastActivityAsync(int sessionId)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session == null) return false;

            session.LastActivityAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------- طرق تنفيذ محادثات المستخدمين ------------------

        // طرق الـ Conversation
        public async Task<Conversation> CreateConversationAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> GetConversationByIdAsync(int conversationId, bool includeParticipants = false)
        {
            if (includeParticipants)
            {
                return await _context.Conversations
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == conversationId);
            }
            else
            {
                return await _context.Conversations.FindAsync(conversationId);
            }
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.LeftAt == null))
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateConversationLastActivityAsync(int conversationId)
        {
            var conversation = await _context.Conversations.FindAsync(conversationId);
            if (conversation == null) return false;

            conversation.LastMessageAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // طرق الـ Message
        public async Task<Message> AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Messages
                .Include(m => m.ReadReceipts)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetConversationMessagesCountAsync(int conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetUnreadMessagesCountAsync(int conversationId, string userId)
        {
            // الحصول على آخر وقت قراءة للمستخدم
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null || participant.LastReadAt == null)
            {
                // إذا لم يقرأ المستخدم أي رسالة، فجميع الرسائل غير مقروءة
                return await _context.Messages
                    .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsDeleted)
                    .CountAsync();
            }

            // عدد الرسائل التي تم إرسالها بعد آخر قراءة وليست من المستخدم نفسه
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId &&
                            m.SenderId != userId &&
                            m.SentAt > participant.LastReadAt &&
                            !m.IsDeleted)
                .CountAsync();
        }

        public async Task<bool> MarkMessagesAsReadAsync(int conversationId, string userId)
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null) return false;

            // تحديث وقت آخر قراءة
            participant.LastReadAt = DateTime.UtcNow;

            // الحصول على الرسائل غير المقروءة
            var unreadMessages = await _context.Messages
                .Where(m => m.ConversationId == conversationId &&
                            m.SenderId != userId &&
                            !m.ReadReceipts.Any(r => r.UserId == userId))
                .ToListAsync();

            // إنشاء إيصالات قراءة للرسائل غير المقروءة
            foreach (var message in unreadMessages)
            {
                _context.MessageReadReceipts.Add(new MessageReadReceipt
                {
                    MessageId = message.Id,
                    UserId = userId,
                    ReadAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddMessageReadReceiptAsync(MessageReadReceipt readReceipt)
        {
            _context.MessageReadReceipts.Add(readReceipt);
            await _context.SaveChangesAsync();
            return true;
        }

        // طرق الـ Participant
        public async Task<bool> AddParticipantAsync(ConversationParticipant participant)
        {
            _context.ConversationParticipants.Add(participant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveParticipantAsync(int conversationId, string userId)
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null) return false;

            // بدلاً من حذف المشارك، نقوم بتحديث LeftAt
            participant.LeftAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(int conversationId)
        {
            return await _context.ConversationParticipants
                .Where(p => p.ConversationId == conversationId && p.LeftAt == null)
                .ToListAsync();
        }

        public async Task<ConversationParticipant> GetParticipantAsync(int conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId && p.LeftAt == null);
        }

        public async Task<IEnumerable<ConversationParticipant>> GetConversationAdminsAsync(int conversationId)
        {
            return await _context.ConversationParticipants
                .Where(p => p.ConversationId == conversationId && p.IsAdmin && p.LeftAt == null)
                .ToListAsync();
        }

        public async Task<bool> IsUserInConversationAsync(int conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId && p.LeftAt == null);
        }
    }
}