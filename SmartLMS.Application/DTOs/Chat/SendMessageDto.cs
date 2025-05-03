using System.ComponentModel.DataAnnotations;

namespace SmartLMS.Application.DTOs.Chat
{
    public class SendMessageDto
    {
        [Required]
        public string Content { get; set; }

        public string Attachment { get; set; }
    }
}