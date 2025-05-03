using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities.Chat
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public bool IsGroupChat { get; set; }

        // العلاقات
        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}