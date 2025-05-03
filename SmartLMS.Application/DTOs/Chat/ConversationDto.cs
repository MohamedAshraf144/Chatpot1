using System;
using System.Collections.Generic;

namespace SmartLMS.Application.DTOs.Chat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public bool IsGroupChat { get; set; }
        public string LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public List<ParticipantDto> Participants { get; set; }
    }
}