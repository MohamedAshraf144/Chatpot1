using System;

namespace SmartLMS.Core.Entities.Chat
{
    public class MessageReadReceipt
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public DateTime ReadAt { get; set; }

        // العلاقات
        public Message Message { get; set; }
        public User User { get; set; }
    }
}