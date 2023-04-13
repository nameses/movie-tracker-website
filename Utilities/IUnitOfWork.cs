using Microsoft.AspNetCore.Mvc;

namespace movie_tracker_website.Utilities
{
    public interface IUnitOfWork
    {
        void UploadImage(IFormFile file);
    }
}