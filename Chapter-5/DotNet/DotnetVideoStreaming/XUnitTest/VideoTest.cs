using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services;
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

        [Fact]
        public async Task TestAddVideoAsync()
        {
            //Arrange
            var path = "SampleVideoPath.mp4";

            //Act
            await _videoService.AddVideoAsync(path);
            var video = await _videoRepository.TableNoTracking.LastOrDefaultAsync();

            //Assert
            Assert.NotNull(video);
            Assert.NotEmpty(video.Path);
            Assert.Equal(path, video.Path);
        }
    }
}