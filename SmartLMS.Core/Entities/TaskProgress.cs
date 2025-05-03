using System;

namespace SmartLMS.Core.Entities
{
    public class TaskProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TaskId { get; set; }
        public string TaskType { get; set; } // "Quiz" or "Assignment"
        public bool IsCompleted { get; set; }
        public int Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
        // يمكن إضافة علاقة للـ Quiz أو Assignment حسب TaskType
    }
}
