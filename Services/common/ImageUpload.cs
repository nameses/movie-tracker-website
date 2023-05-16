using Microsoft.AspNetCore.Hosting;
using movie_tracker_website.Utilities;

namespace movie_tracker_website.Services.common
{
    public class ImageUpload : IImageUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageUpload(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UploadImage(IFormFile file)
        {
            if (file.Length > 0)
            {
                string filepath = EnsureFileName(file.FileName);

                using (var stream = File.Create(filepath))
                {
                    file.CopyToAsync(stream);
                }
                return Path.GetFileName(filepath);
            }
            else throw new FileLoadException();
        }

        private string EnsureFileName(string filename)
        {
            string extension = Path.GetExtension(filename);

            string path = _webHostEnvironment.WebRootPath + "\\uploads\\";
            string fullPath = path + Path.ChangeExtension(Path.GetRandomFileName(), extension);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return fullPath;
        }
    }
}