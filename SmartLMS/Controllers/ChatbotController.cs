using Microsoft.AspNetCore.Mvc;
using SmartLMS.Application.DTOs;
using SmartLMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatbotController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            try
            {
                return Ok("ChatbotController يعمل بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في نقطة النهاية Test: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatResponseDto>> SendMessage([FromBody] ChatRequestDto request, [FromQuery] string userId = "test-user")
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("الرسالة مطلوبة");
            }

            try
            {
                var userIdToUse = userId ?? "test-user";
                Console.WriteLine($"Processing message: {request.Message} for user: {userIdToUse}");

                var response = await _chatService.ProcessMessageAsync(request, userIdToUse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    source = ex.Source,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("sessions")]
        public async Task<ActionResult<IEnumerable<ChatSessionDto>>> GetUserSessions([FromQuery] string userId = "test-user")
        {
            try
            {
                var sessions = await _chatService.GetUserSessionsAsync(userId ?? "test-user");
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserSessions: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("sessions/{sessionId}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetSessionMessages(int sessionId)
        {
            try
            {
                var messages = await _chatService.GetSessionMessagesAsync(sessionId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSessionMessages: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}