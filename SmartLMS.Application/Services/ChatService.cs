using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SmartLMS.Application.DTOs;
using SmartLMS.Application.DTOs.Chat;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Entities.Chat;
using SmartLMS.Core.Enums;
using SmartLMS.Core.Interfaces;

namespace SmartLMS.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatbotService _chatbotService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ChatService(
            IChatRepository chatRepository,
            IChatbotService chatbotService,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _chatRepository = chatRepository;
            _chatbotService = chatbotService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // Chatbot جلسات المحادثة مع
        public async Task<IEnumerable<ChatSessionDto>> GetUserSessionsAsync(string userId)
        {
            var sessions = await _chatRepository.GetUserSessionsAsync(userId);
            return sessions.Select(s => new ChatSessionDto
            {
                Id = s.Id,
                Title = s.Title,
                CreatedAt = s.CreatedAt,
                LastActivityAt = s.LastActivityAt
            });
        }

        public async Task<IEnumerable<ChatMessageDto>> GetSessionMessagesAsync(int sessionId)
        {
            var messages = await _chatRepository.GetSessionMessagesAsync(sessionId);
            return messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                Content = m.Content,
                Timestamp = m.Timestamp,
                IsFromBot = m.IsFromBot
            });
        }

        public async Task<ChatResponseDto> ProcessMessageAsync(ChatRequestDto request, string userId)
        {
            ChatSession session;
            if (request.SessionId.HasValue)
            {
                session = await _chatRepository.GetSessionByIdAsync(request.SessionId.Value);
                if (session == null || session.UserId != userId)
                    throw new UnauthorizedAccessException("لا يمكن الوصول إلى جلسة المحادثة هذه");
            }
            else
            {
                string title = request.Message.Length > 30 ? request.Message.Substring(0, 27) + "..." : request.Message;
                session = await _chatRepository.CreateSessionAsync(userId, title);
            }

            var userMessage = new ChatMessage
            {
                UserId = userId,
                Content = request.Message,
                Timestamp = DateTime.UtcNow,
                IsFromBot = false,
                ChatSessionId = session.Id
            };
            await _chatRepository.SaveMessageAsync(userMessage);

            var conversationHistory = await _chatRepository.GetSessionMessagesAsync(session.Id);
            string botResponse = await _chatbotService.GetResponseAsync(
                request.Message,
                conversationHistory.TakeLast(10)
            );

            var botMessage = new ChatMessage
            {
                UserId = userId,
                Content = botResponse,
                Timestamp = DateTime.UtcNow,
                IsFromBot = true,
                ChatSessionId = session.Id
            };
            await _chatRepository.SaveMessageAsync(botMessage);
            await _chatRepository.UpdateSessionLastActivityAsync(session.Id);

            return new ChatResponseDto
            {
                Message = botResponse,
                SessionId = session.Id,
                Timestamp = botMessage.Timestamp
            };
        }

        // محادثات بين المستخدمين
        public async Task<ConversationDto> CreateConversationAsync(string userId, List<string> participantIds, string title = null)
        {
            var allParticipantIds = new List<string>(participantIds);
            if (!allParticipantIds.Contains(userId))
                allParticipantIds.Add(userId);

            var isGroupChat = allParticipantIds.Count > 2;

            if (isGroupChat && string.IsNullOrEmpty(title))
            {
                var participants = await _userRepository.GetUsersByIdsAsync(allParticipantIds);
                title = string.Join(", ", participants.Select(p => p.Name).Take(3));
                if (allParticipantIds.Count > 3)
                    title += $" وآخرون ({allParticipantIds.Count - 3})";
            }

            var conversation = new Conversation
            {
                Title = title,
                IsGroupChat = isGroupChat,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            var createdConversation = await _chatRepository.CreateConversationAsync(conversation);

            foreach (var participantId in allParticipantIds)
            {
                await _chatRepository.AddParticipantAsync(new ConversationParticipant
                {
                    ConversationId = createdConversation.Id,
                    UserId = participantId,
                    JoinedAt = DateTime.UtcNow,
                    IsAdmin = participantId == userId
                });
            }

            var systemMessage = new Message
            {
                ConversationId = createdConversation.Id,
                SenderId = userId,
                Content = "تم إنشاء المحادثة",
                Type = MessageType.SystemMessage,
                SentAt = DateTime.UtcNow
            };

            await _chatRepository.AddMessageAsync(systemMessage);
            return await GetConversationByIdAsync(createdConversation.Id, userId);
        }

        public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(string userId)
        {
            var conversations = await _chatRepository.GetUserConversationsAsync(userId);
            var conversationDtos = new List<ConversationDto>();

            foreach (var conversation in conversations)
            {
                var unreadCount = await _chatRepository.GetUnreadMessagesCountAsync(conversation.Id, userId);
                var participants = await _chatRepository.GetConversationParticipantsAsync(conversation.Id);
                var participantUsers = new List<ParticipantDto>();

                foreach (var participant in participants)
                {
                    var user = await _userRepository.GetUserByIdAsync(participant.UserId);
                    if (user != null)
                    {
                        var participantDto = _mapper.Map<ParticipantDto>(user);
                        participantDto.IsAdmin = participant.IsAdmin;
                        participantUsers.Add(participantDto);
                    }
                }

                var messages = await _chatRepository.GetConversationMessagesAsync(conversation.Id, 1, 1);
                var lastMessage = messages.FirstOrDefault()?.Content ?? "";

                var conversationDto = _mapper.Map<ConversationDto>(conversation);
                conversationDto.Participants = participantUsers;
                conversationDto.UnreadCount = unreadCount;
                conversationDto.LastMessage = lastMessage;

                conversationDtos.Add(conversationDto);
            }

            return conversationDtos.OrderByDescending(c => c.LastMessageAt);
        }

        public async Task<ConversationDto> GetConversationByIdAsync(int conversationId, string userId)
        {
            var conversation = await _chatRepository.GetConversationByIdAsync(conversationId, true);
            if (conversation == null)
                return null;

            var participant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null)
                return null;

            var unreadCount = await _chatRepository.GetUnreadMessagesCountAsync(conversationId, userId);

            var participantDtos = new List<ParticipantDto>();
            foreach (var p in conversation.Participants)
            {
                var user = await _userRepository.GetUserByIdAsync(p.UserId);
                if (user != null)
                {
                    var participantDto = _mapper.Map<ParticipantDto>(user);
                    participantDto.IsAdmin = p.IsAdmin;
                    participantDtos.Add(participantDto);
                }
            }

            var messages = await _chatRepository.GetConversationMessagesAsync(conversationId, 1, 1);
            var lastMessage = messages.FirstOrDefault()?.Content ?? "";

            var conversationDto = _mapper.Map<ConversationDto>(conversation);
            conversationDto.Participants = participantDtos;
            conversationDto.UnreadCount = unreadCount;
            conversationDto.LastMessage = lastMessage;

            return conversationDto;
        }

        public async Task<MessagesResponseDto> GetConversationMessagesAsync(int conversationId, string userId, int page = 1, int pageSize = 20)
        {
            var isParticipant = await _chatRepository.IsUserInConversationAsync(conversationId, userId);
            if (!isParticipant)
                return null;

            var totalMessages = await _chatRepository.GetConversationMessagesCountAsync(conversationId);
            var messages = await _chatRepository.GetConversationMessagesAsync(conversationId, page, pageSize);

            var messageDtos = new List<MessageDto>();
            foreach (var message in messages)
            {
                var sender = await _userRepository.GetUserByIdAsync(message.SenderId);
                var messageDto = _mapper.Map<MessageDto>(message);

                if (sender != null)
                {
                    messageDto.SenderName = sender.Name;
                    messageDto.SenderProfileImage = sender.ProfileImage;
                }

                messageDto.IsRead = message.ReadReceipts.Any(r => r.UserId == userId);
                messageDtos.Add(messageDto);
            }

            await _chatRepository.MarkMessagesAsReadAsync(conversationId, userId);

            return new MessagesResponseDto
            {
                Messages = messageDtos,
                TotalCount = totalMessages,
                PageNumber = page,
                PageSize = pageSize,
                HasMore = (page * pageSize) < totalMessages
            };
        }

        public async Task<MessageDto> SendMessageAsync(int conversationId, string senderId, string content, string attachment = null)
        {
            var isParticipant = await _chatRepository.IsUserInConversationAsync(conversationId, senderId);
            if (!isParticipant)
                return null;

            var messageType = MessageType.Text;

            if (!string.IsNullOrEmpty(attachment))
            {
                var extension = System.IO.Path.GetExtension(attachment).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                    messageType = MessageType.Image;
                else
                    messageType = MessageType.File;
            }

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                Type = messageType,
                SentAt = DateTime.UtcNow,
                Attachment = attachment,
                IsEdited = false,
                IsDeleted = false
            };

            var addedMessage = await _chatRepository.AddMessageAsync(message);
            await _chatRepository.UpdateConversationLastActivityAsync(conversationId);

            var readReceipt = new MessageReadReceipt
            {
                MessageId = addedMessage.Id,
                UserId = senderId,
                ReadAt = DateTime.UtcNow
            };
            await _chatRepository.AddMessageReadReceiptAsync(readReceipt);

            var sender = await _userRepository.GetUserByIdAsync(senderId);
            var messageDto = _mapper.Map<MessageDto>(addedMessage);

            if (sender != null)
            {
                messageDto.SenderName = sender.Name;
                messageDto.SenderProfileImage = sender.ProfileImage;
            }

            messageDto.IsRead = true;
            return messageDto;
        }

        public async Task<bool> MarkConversationAsReadAsync(int conversationId, string userId)
        {
            return await _chatRepository.MarkMessagesAsReadAsync(conversationId, userId);
        }

        public async Task<bool> AddParticipantToConversationAsync(int conversationId, string userId, string participantId)
        {
            var participant = await _chatRepository.GetParticipantAsync(conversationId, userId);
            if (participant == null || !participant.IsAdmin)
                return false;

            var conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
            if (conversation == null)
                return false;

            var userToAdd = await _userRepository.GetUserByIdAsync(participantId);
            if (userToAdd == null)
                return false;

            var existingParticipant = await _chatRepository.GetParticipantAsync(conversationId, participantId);
            if (existingParticipant != null)
                return true;

            var newParticipant = new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = participantId,
                JoinedAt = DateTime.UtcNow,
                IsAdmin = false
            };

            var result = await _chatRepository.AddParticipantAsync(newParticipant);

            if (result)
            {
                var systemMessage = new Message
                {
                    ConversationId = conversationId,
                    SenderId = userId,
                    Content = $"{userToAdd.Name} تمت إضافته إلى المحادثة",
                    Type = MessageType.SystemMessage,
                    SentAt = DateTime.UtcNow
                };

                await _chatRepository.AddMessageAsync(systemMessage);
                await _chatRepository.UpdateConversationLastActivityAsync(conversationId);
            }

            return result;
        }

        public async Task<bool> RemoveParticipantFromConversationAsync(int conversationId, string userId, string participantToRemoveId)
        {
            var participant = await _chatRepository.GetParticipantAsync(conversationId, userId);
            if (participant == null || (!participant.IsAdmin && userId != participantToRemoveId))
                return false;

            var conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
            if (conversation == null)
                return false;

            var participantToRemove = await _chatRepository.GetParticipantAsync(conversationId, participantToRemoveId);
            if (participantToRemove == null)
                return false;

            if (participantToRemove.IsAdmin && conversation.IsGroupChat)
            {
                var admins = await _chatRepository.GetConversationAdminsAsync(conversationId);
                if (admins.Count() == 1 && admins.First().UserId == participantToRemoveId)
                    return false;
            }

            var userToRemove = await _userRepository.GetUserByIdAsync(participantToRemoveId);
            var result = await _chatRepository.RemoveParticipantAsync(conversationId, participantToRemoveId);

            if (result)
            {
                var messageContent = userId == participantToRemoveId
                    ? $"{userToRemove.Name} غادر المحادثة"
                    : $"{userToRemove.Name} تمت إزالته من المحادثة";

                var systemMessage = new Message
                {
                    ConversationId = conversationId,
                    SenderId = userId,
                    Content = messageContent,
                    Type = MessageType.SystemMessage,
                    SentAt = DateTime.UtcNow
                };

                await _chatRepository.AddMessageAsync(systemMessage);
                await _chatRepository.UpdateConversationLastActivityAsync(conversationId);
            }

            return result;
        }
    }
}