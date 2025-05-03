using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace SmartLMS.Application.DTOs.Chat
{
    public class AddParticipantDto
    {
        [Required]
        public string ParticipantId { get; set; }
    }
}