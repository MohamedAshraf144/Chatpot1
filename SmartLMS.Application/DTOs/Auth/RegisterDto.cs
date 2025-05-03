using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace SmartLMS.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; } = "Student"; // القيمة الافتراضية هي "طالب"

        public string Phone { get; set; }

        public string Gender { get; set; }

        public string Region { get; set; }

        public string ProfileImage { get; set; }
    }
}

