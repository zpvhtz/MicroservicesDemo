using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace MicroserviceDemo_Chap3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VideoController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public PhysicalFileResult GetVideo()
        {
            //var path = @"videos/SampleVideo.mp4";
            //return new PhysicalFileResult(path, "video/mp4");

            var videoPath = @"videos/SampleVideo.mp4";
            //var videoPath = "SampleVideo.mp4";
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            var fullPath = $@"{contentRootPath}/{videoPath}";

            //var videoPath = @"wwwroot\DemoSources\SampleVideo.mp4";
            //var fullPath = $@"{Directory.GetCurrentDirectory()}\{videoPath}";

            return PhysicalFile(fullPath, "video/mp4", enableRangeProcessing: true);
        }
    }
}