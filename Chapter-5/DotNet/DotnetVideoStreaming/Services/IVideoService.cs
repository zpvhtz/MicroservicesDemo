using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IVideoService
    {
        Task<List<Videos>> GetAllVideosAsync();
        Task<Videos> GetVideoByIdAsync(int videoId);
        Task AddVideoAsync(string path);
        Task UpdateVideoAsync(int id, string path);
    }
}