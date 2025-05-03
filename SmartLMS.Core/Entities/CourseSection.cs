using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class CourseSection
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Course Course { get; set; }
        public ICollection<CourseContent> Contents { get; set; }
    }
}