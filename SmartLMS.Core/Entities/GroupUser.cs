using System;

namespace SmartLMS.Core.Entities
{
    public class GroupUser
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Group Group { get; set; }
        public User User { get; set; }
    }
}