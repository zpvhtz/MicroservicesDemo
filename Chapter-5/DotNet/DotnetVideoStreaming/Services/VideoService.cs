using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task AddVideoAsync(string path)
        {
            var video = new Videos
            {
                Path = path
            };

            await _videoRepository.AddAsync(video);
        }

        public async Task UpdateVideoAsync(int id, string path)
        {
            var video = await _videoRepository.Table.Where(p => p.Id == id)
                                                    .FirstOrDefaultAsync();

            if(video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }

            await _videoRepository.UpdateAsync(video);
        }
    }
}