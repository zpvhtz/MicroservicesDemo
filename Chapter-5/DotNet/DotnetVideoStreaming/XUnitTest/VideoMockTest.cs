using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class VideoMockTest
    {
        [Trait("Video Mocking Test", "Adding")]
        [Fact(DisplayName = "Adding Video xUT")]
        public async Task TestAddVideoAsync()
        {
            // create mock version
            var mockDependency = new Mock<IRepository<Videos>>();

            // set up mock version's method
            //mockDependency.Setup(p => p.TableNoTracking.Where((It.IsAny<int>()))
            //                           .Returns("SampleVideo.mp4");

            // create thing being tested with a mock dependency
            var sut = new VideoService(mockDependency.Object);

            //Arange: Set up the any variables and objects necessary.
            var path = "SampleVideoPathMockTest.mp4";

            //Act: Call the method being tested, passing any parameters needed
            var video = await sut.AddVideoAsync(path);

            //Assert: Verify expected results
            Assert.NotNull(video);
            Assert.NotEmpty(video.Path);
            Assert.Equal(path, video.Path);
        }

        [Trait("Video Mocking Test", "Updating")]
        [Theory(DisplayName = "Updating Video xUT")]
        [InlineData(1, "SampleVideoPathMockTest.mp4")]
        public async Task TestUpdateVideoAsync(int expectedVideoId, string expectedVideoPath)
        {
            // create mock version
            var mockDependency = new Mock<IRepository<Videos>>();

            // set up mock version's method
            //mockDependency.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Videos());
            mockDependency.Setup(p => p.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Videos() { Id = 1, Path = "test.mp4" });

            // create thing being tested with a mock dependency
            var sut = new VideoService(mockDependency.Object);

            //Arange: Set up the any variables and objects necessary.
            var path = "SampleVideoPathMockTest.mp4";

            //Act: Call the method being tested, passing any parameters needed
            var video = await sut.UpdateVideoAsync(1, path);

            //Assert: Verify expected results
            Assert.NotNull(video);
            Assert.NotEmpty(video.Path);
            Assert.Equal(expectedVideoPath, video.Path);
            Assert.Equal(expectedVideoId, video.Id);
        }
    }
}