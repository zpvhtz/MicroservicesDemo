using Microsoft.AspNetCore.Mvc;
using Services;
using System.IO;
using System.Threading.Tasks;

namespace DotnetVideoStorage.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        public string CreateTempPath(string extension = ".txt") => Path.ChangeExtension(Path.GetTempFileName(), extension);
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet("GetVideo")]
        public async Task<ActionResult> GetVideo(string path)
        {
            System.Console.WriteLine("GetVideo API from Storage");
            var fileStream = await _videoService.GetVideo(path);
            //return Ok(fileStream.ToArray());
            return new FileContentResult(fileStream.ToArray(), "video/mp4");
        }
    }
}