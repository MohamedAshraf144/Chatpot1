using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class CourseContent
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int? CourseSectionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string VideoUrl { get; set; }
        public int? Duration { get; set; }
        public string ContentUrl { get; set; }
        public bool IsFree { get; set; }
        public int Order { get; set; }
        public int SectionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Course Course { get; set; }
        public CourseSection Section { get; set; }
        public ICollection<ContentProgress> ContentProgresses { get; set; }
    }
}
