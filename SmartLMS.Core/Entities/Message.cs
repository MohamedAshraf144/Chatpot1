using SmartLMS.Core.Enums;

namespace SmartLMS.Core.Entities.Chat
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }
        public string Attachment { get; set; }
        // العلاقات
        public Conversation Conversation { get; set; }
        public User Sender { get; set; }
        public ICollection<MessageReadReceipt> ReadReceipts { get; set; } = new List<MessageReadReceipt>();
    }
}