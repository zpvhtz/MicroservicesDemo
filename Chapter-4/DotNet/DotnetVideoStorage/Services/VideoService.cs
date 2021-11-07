using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class VideoService : IVideoService
    {
        public async Task<MemoryStream> GetVideo(string path)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var containerName = "videoscontainer";

            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            try
            {
                BlobClient blob = container.GetBlobClient(path);

                MemoryStream fileStream = new MemoryStream();
                await blob.DownloadToAsync(fileStream);

                return fileStream;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}