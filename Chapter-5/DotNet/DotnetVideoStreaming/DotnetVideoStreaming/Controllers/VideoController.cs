using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DotnetVideoStreaming.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet("GetVideoById")]
        public async Task<ActionResult> GetVideoById(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            return Ok(video.Path);
        }

        [HttpGet("AddVideoByPath")]
        public async Task<ActionResult> AddVideoByPath(string path)
        {
            var video = await _videoService.AddVideoAsync(path);
            return Ok(video);
        }

        [HttpGet("UpdateVideoById")]
        public async Task<ActionResult> UpdateVideoById(int id, string path)
        {
            var video = await _videoService.UpdateVideoAsync(id, path);
            return Ok(video);
        }

        [HttpGet("GetVideo")]
        public async Task<ActionResult> GetVideo(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);

            if (video == null)
            {
                throw new ArgumentException("Video not found");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://host.docker.internal:5002");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"/video/getvideo?path={video.Path}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var fileStreamArray = await response.Content.ReadAsByteArrayAsync();
                        return new FileContentResult(fileStreamArray, "video/mp4");
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                }
                else
                {
                    throw new ArgumentException("Cannot connect via url");
                }
            }
        }

        [HttpGet("TestSonarScannerCodeSmell")]
        public async Task<ActionResult> TestSonarScanner()
        {
            string unusedString = "This is a string";

            return Ok();
        }

        [HttpGet("TestSonarScannerBugs1")]
        public ActionResult TestSonarScannerBugs1()
        {
            Videos video = null;
            video.Path = "SamplePath";

            return Ok(video);
        }

        [HttpGet("TestSonarScannerBugs2")]
        public ActionResult TestSonarScannerBugs2()
        {
            var alwaysFalse = false;

            if (alwaysFalse)
                alwaysFalse = true;

            return Ok();
        }
    }
}