using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class VideoTest
    {
        private readonly VideoStreamingContext _context;
        private readonly EfRepository<Videos> _videoRepository;
        private readonly VideoService _videoService;

        public VideoTest()
        {
            var options = new DbContextOptionsBuilder<VideoStreamingContext>()
                        .UseInMemoryDatabase(databaseName: "TestNewListDb").Options;

            _context = new VideoStreamingContext(options);

            _videoRepository = new EfRepository<Videos>(_context);
            _videoService = new VideoService(_videoRepository);
        }

        [Trait("Video Test", "Adding")]
        [Fact(DisplayName = "Adding Video xUT")]
        public async Task TestAddVideoAsync()
        {
            //Arrange: Set up the any variables and objects necessary.
            var path = "SampleVideoPathTest.mp4";

            //Act: Call the method being tested, passing any parameters needed
            var video = await _videoService.AddVideoAsync(path);

            //Assert: Verify expected results
            Assert.NotNull(video);
            Assert.NotEmpty(video.Path);
            Assert.Equal(1, video.Id);
        }

        [Trait("Video Test", "Updating")]
        [Fact(DisplayName = "Updating Video xUT")]
        //[Theory(DisplayName = "Updating Video xUT")]
        //[InlineData(2)]
        //public async Task TestUpdateVideoAsync(int videoId)
        public async Task TestUpdateVideoAsync()
        {
            //Arange: Set up the any variables and objects necessary.
            var path = "SampleVideoPathTest.mp4 edited!";

            //Act: Call the method being tested, passing any parameters needed
            var video = await _videoService.UpdateVideoAsync(1, path);

            //Assert: Verify expected results
            Assert.NotNull(video);
            Assert.NotEmpty(video.Path);
            Assert.Equal(path, video.Path);
        }
    }
}