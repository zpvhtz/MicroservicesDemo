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
            await _videoService.AddVideoAsync(path);
            return Ok();
        }

        [HttpGet("GetVideo")]
        public async Task<ActionResult> GetVideo(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);

            if (video == null)
            {
                throw new Exception("Video not found");
            }

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://localhost:5002/");
                //client.BaseAddress = new Uri("http://localhost:5002/");
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
                        //return Ok(await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    throw new Exception("Cannot connect via url");
                }
            }
        }
    }
}