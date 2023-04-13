using Microsoft.AspNetCore.Mvc;

namespace movie_tracker_website.Utilities
{
    public interface IImageUpload
    {
        string UploadImage(IFormFile file);
    }
}