using Microsoft.AspNetCore.Mvc;
using Services;
using System.IO;

namespace DotnetVideoStreaming.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQController : ControllerBase
    {
        private readonly IRabbitService _rabbitService;
        public RabbitMQController(IRabbitService rabbitService)
        {
            _rabbitService = rabbitService;
        }

        [HttpGet("SendMessage")]
        public IActionResult SendMessage()
        {
            var content = new FileStream(@"videos/SampleVideo.mp4", FileMode.Open, FileAccess.Read, FileShare.Read);
            var response = File(content, "video/mp4");

            _rabbitService.SendMessage();

            return response;
        }

        [HttpGet("SendMultipleMessages")]
        public IActionResult SendMultipleMessages()
        {
            var content = new FileStream(@"videos/SampleVideo.mp4", FileMode.Open, FileAccess.Read, FileShare.Read);
            var response = File(content, "video/mp4");

            _rabbitService.SendMultipleMessages();

            return response;
        }
    }
}
