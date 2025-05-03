using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int TotalMarks { get; set; }
        public bool IsRequired { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Course Course { get; set; }
        public ICollection<AssignmentSubmission> Submissions { get; set; }
    }
}
