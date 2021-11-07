using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public interface IVideoService
    {
        Task<MemoryStream> GetVideo(string path);
    }
}