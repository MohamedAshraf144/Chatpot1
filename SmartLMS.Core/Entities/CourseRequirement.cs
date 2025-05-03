using System;

namespace SmartLMS.Core.Entities
{
    public class CourseRequirement
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Requirement { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Course Course { get; set; }
    }
}