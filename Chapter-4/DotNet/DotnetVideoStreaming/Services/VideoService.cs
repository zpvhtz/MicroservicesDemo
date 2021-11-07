using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class VideoService : IVideoService
    {
        private readonly IRepository<Videos> _videoRepository;

        public VideoService(IRepository<Videos> videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public async Task<List<Videos>> GetAllVideosAsync()
        {
            return await _videoRepository.TableNoTracking.ToListAsync();
        }

        public async Task<Videos> GetVideoByIdAsync(int videoId)
        {
            return await _videoRepository.TableNoTracking.Where(p => p.Id == videoId)
                                                         .FirstOrDefaultAsync();
        }
    }
}