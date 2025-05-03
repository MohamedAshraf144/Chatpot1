using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseImage { get; set; }
        public string CourseLevel { get; set; }
        public int? MajorId { get; set; }
        public int InstructorId { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int? LessonNumber { get; set; }
        public string CourseHours { get; set; }
        public bool IsArchived { get; set; }
        public string CourseCode { get; set; }
        public float? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Major Major { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<CourseSection> Sections { get; set; }
        public ICollection<CourseContent> Contents { get; set; }
        public ICollection<CourseRequirement> Requirements { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
    }
}
