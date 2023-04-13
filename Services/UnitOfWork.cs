using Microsoft.AspNetCore.Hosting;
using movie_tracker_website.Utilities;

namespace movie_tracker_website.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UnitOfWork(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async void UploadImage(IFormFile file)
        {
            long totalBytes = file.Length;
            string filename = file.FileName.Trim('"');
            filename = EnsureFileName(filename);
            byte[] buffer = new byte[16 * 1024];
            using (FileStream output = File.Create(filename))
            {
                using (Stream input = file.OpenReadStream())
                {
                    int readBytes;
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, readBytes);
                        totalBytes += readBytes;
                    }
                }
            }
        }

        private string EnsureFileName(string filename)
        {
            string path = _webHostEnvironment.WebRootPath + "\\uploads\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return (path + filename);
        }
    }
}