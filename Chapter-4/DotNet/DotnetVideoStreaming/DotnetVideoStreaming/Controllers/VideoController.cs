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

        [HttpGet("GetVideo")]
        public async Task<ActionResult> GetVideo(int id)
        {
            System.Console.WriteLine("GetVideo API from Streaming");
            var video = await _videoService.GetVideoByIdAsync(id);

            if(video == null)
            {
                throw new Exception("Video not found");
            }

            //FileStreamResult response;
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://localhost:5002/");
                //client.BaseAddress = new Uri("http://localhost:5002/");
                client.BaseAddress = new Uri("http://host.docker.internal:5002");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"/video/getvideo?path={video.Path}");
                //var response = await client.GetStreamAsync($"/video/getvideo?path={video.Path}");

                //try
                //{
                //    string host = Environment.GetEnvironmentVariable("VIDEO_STORAGE_HOST");
                //    string port = Environment.GetEnvironmentVariable("VIDEO_STORAGE_PORT");
                //    var res = await client.GetStreamAsync($"http://{host}:{port}/Video/GetVideo?path={video.Path}");
                //    response = File(res, "video/mp4");
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(ex.Message);
                //}
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
            
            //return response;
        }
    }
}