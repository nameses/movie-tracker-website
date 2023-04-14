using System.ComponentModel.DataAnnotations;

namespace movie_tracker_website.ViewModels
{
    public class AppUserViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string? ImagePath { get; set; }
    }
}