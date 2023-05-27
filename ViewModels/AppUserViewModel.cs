using movie_tracker_website.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace movie_tracker_website.ViewModels
{
    public class AppUserViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string? ImagePath { get; set; }

        public static AppUserViewModel convertToViewModel(AppUser user)
        {
            return new AppUserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                ImagePath = user.ImagePath
            };
        }
    }
}