// SmartLMS.Application/DTOs/Chat/CreateConversationDto.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartLMS.Application.DTOs.Chat
{
    public class CreateConversationDto
    {
        [Required]
        public List<string> ParticipantIds { get; set; }

        public string Title { get; set; } // اختياري لمحادثات المجموعة
    }
}