using AutoMapper;
using SmartLMS.Application.DTOs;
using SmartLMS.Application.DTOs.Chat;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Entities.Chat;

namespace SmartLMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // تعيينات الشات بوت
            CreateMap<ChatMessage, ChatMessageDto>();
            CreateMap<ChatSession, ChatSessionDto>();

            // تعيينات المحادثات بين المستخدمين
            CreateMap<Conversation, ConversationDto>();
            CreateMap<Message, MessageDto>();
            CreateMap<ConversationParticipant, ParticipantDto>();
            CreateMap<User, ParticipantDto>();
        }
    }
}