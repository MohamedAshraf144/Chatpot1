using System;

namespace SmartLMS.Core.Entities
{
    public class UserAccess
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
    }
}