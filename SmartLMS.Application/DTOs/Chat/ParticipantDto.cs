using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLMS.Application.DTOs.Chat
{
    public class ParticipantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string Role { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAdmin { get; set; }
    }
}