using System;

namespace SmartLMS.Core.Entities.Chat
{
    public class ConversationParticipant
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string UserId { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? LastReadAt { get; set; }

        // العلاقات
        public Conversation Conversation { get; set; }
        public User User { get; set; }
    }
}