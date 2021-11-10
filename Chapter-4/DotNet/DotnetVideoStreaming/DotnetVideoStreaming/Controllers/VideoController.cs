using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            {//var response = await client.GetStreamAsync($"/video/getvideo?path={video.Path}");

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

                //client.BaseAddress = new Uri("https://localhost:5002/");
                //client.BaseAddress = new Uri("http://localhost:5002/");
                client.BaseAddress = new Uri("http://host.docker.internal:5002");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"/video/getvideo?path={video.Path}");

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

        [HttpGet("SendSoapMessage")]
        public async Task<ActionResult> SendSoapMessage()
        {
            string url = "https://localhost:44371/ServiceAbcXyz.asmx";

            //string xmlSOAP1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            //        <soap:Body>    
            //            <TestCustomModel xmlns=""http://tempuri.org/"">
            //              <s> This is Test String </s>
            //            </TestCustomModel >
            //        </soap:Body>
            //    </soap:Envelope>";

            string testString = "This is a test string";

            string xmlSOAP = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                    <soap:Body>    
                        <TestCustomModel xmlns=""http://tempuri.org/"">
                          <s> {0} </s>
                        </TestCustomModel >
                    </soap:Body>
                </soap:Envelope>", testString);

            try
            {
                using (HttpContent content = new StringContent(xmlSOAP, Encoding.UTF8, "text/xml"))
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        HttpClient httpClient = new HttpClient();
                        request.Headers.Add("SOAPAction", "");
                        request.Content = content;
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/soap+xml"));
                        using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            //response.EnsureSuccessStatusCode(); // throws an Exception if 404, 500, etc.
                            var result = await response.Content.ReadAsStringAsync();
                            return Ok(result);
                        }
                    }
                }
                //string result = await PostSOAPRequestAsync(url, xmlSOAP);
                //Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ok("");
        }
    }
}