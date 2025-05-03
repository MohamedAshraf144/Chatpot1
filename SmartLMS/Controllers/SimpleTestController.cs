using Microsoft.AspNetCore.Mvc;

namespace SmartLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API يعمل بنجاح!" });
        }

        [HttpPost]
        public IActionResult Post([FromBody] object data)
        {
            return Ok(new { message = "تم استلام البيانات بنجاح", data });
        }
    }
}