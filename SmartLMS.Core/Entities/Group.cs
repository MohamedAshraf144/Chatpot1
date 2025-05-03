using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int? GroupAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Course Course { get; set; }
        public ICollection<GroupUser> Users { get; set; }
    }
}