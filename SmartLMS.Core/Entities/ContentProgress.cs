using System;

namespace SmartLMS.Core.Entities
{
    public class ContentProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ContentId { get; set; }
        public bool IsCompleted { get; set; }
        public float ProgressPercent { get; set; }
        public DateTime LastAccessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
        public CourseContent Content { get; set; }
    }
}