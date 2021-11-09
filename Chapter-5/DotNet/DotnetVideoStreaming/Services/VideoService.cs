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

        public string GetVideoPathById(int videoId)
        {
            var video = _videoRepository.TableNoTracking.Where(p => p.Id == videoId)
                                                        .FirstOrDefault();

            if (video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }

            return video.Path;
        }

        public async Task<string> GetVideoPathByIdAsync(int videoId)
        {
            var video = await _videoRepository.TableNoTracking.Where(p => p.Id == videoId)
                                                              .FirstOrDefaultAsync();

            if (video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }

            return video.Path;
        }

        public async Task<Videos> AddVideoAsync(string path)
        {
            var video = new Videos
            {
                Path = path
            };

            await _videoRepository.AddAsync(video);

            return video;
        }

        public async Task<Videos> UpdateVideoAsync(int id, string path)
        {
            var video = await _videoRepository.GetByIdAsync(id);

            if (video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }

            video.Path = path;

            await _videoRepository.UpdateAsync(video);

            return video;
        }
    }
}