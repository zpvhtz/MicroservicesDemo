using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroserviceDemo_Chap2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        //[HttpGet]
        //public ActionResult Get()
        //{
        //    return Ok("Hello World!");
        //}

        [HttpGet]
        public async Task<ActionResult> GetVideo()
        {
            //var path = @"E:\Demo\MicroservicesDemo\Chapter-2\DotNet\MicroserviceDemo-Chap2\MicroserviceDemo-Chap2\wwwroot\DemoSources\SampleVideo.mp4";
            var videoPath = @"wwwroot\DemoSources\SampleVideo.mp4";
            var fullPath = $@"{Directory.GetCurrentDirectory()}\{videoPath}";

            return PhysicalFile(fullPath, "video/mp4", enableRangeProcessing: true);

            //var memory = new MemoryStream();
            //using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 65536, FileOptions.Asynchronous | FileOptions.SequentialScan))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;
            //return File(memory, "application/octet-stream", Path.GetFileName(path));
        }
    }
}