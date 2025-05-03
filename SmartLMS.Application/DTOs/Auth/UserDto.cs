using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLMS.Application.DTOs.Auth
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public List<string> Roles { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Region { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}