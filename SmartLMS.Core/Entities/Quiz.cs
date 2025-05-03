using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Quiz
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int PassingScore { get; set; }
        public string Status { get; set; }
        public string Instructions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Course Course { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}