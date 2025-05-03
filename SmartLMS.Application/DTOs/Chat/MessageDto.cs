using System;

namespace SmartLMS.Application.DTOs.Chat
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderProfileImage { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }
        public string Attachment { get; set; }
        public bool IsRead { get; set; }
    }
}