using System;

namespace SmartLMS.Core.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public bool IsCompleted { get; set; }
        public int ProgressPercent { get; set; }
        public bool IsRegisteredStudent { get; set; }
        public bool IsMaxStudent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
        public Course Course { get; set; }
    }
}
