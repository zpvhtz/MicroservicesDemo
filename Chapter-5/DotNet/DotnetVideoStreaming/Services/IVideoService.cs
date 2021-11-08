using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IVideoService
    {
        Task<List<Videos>> GetAllVideosAsync();
        Task<Videos> GetVideoByIdAsync(int videoId);
        string GetVideoPathById(int videoId);
        Task<string> GetVideoPathByIdAsync(int videoId);
        Task<Videos> AddVideoAsync(string path);
        Task<Videos> UpdateVideoAsync(int id, string path);
    }
}