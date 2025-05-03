using System;

namespace SmartLMS.Core.Entities
{
    public class Specialty
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }

        // العلاقات
        public Instructor Instructor { get; set; }
    }
}
