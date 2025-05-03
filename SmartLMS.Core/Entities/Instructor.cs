using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Instructor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string EmailVerified { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public string Status { get; set; }
        public decimal? Salary { get; set; }
        public decimal? SalaryDeduction { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Specialty> Specialties { get; set; }
    }
}
