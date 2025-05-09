﻿using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Major
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public ICollection<Course> Courses { get; set; }
    }
}