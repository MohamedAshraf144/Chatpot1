using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLMS.Application.DTOs
{
    public class ChatRequestDto
    {
        public string Message { get; set; }
        public int? SessionId { get; set; }
    }
}
