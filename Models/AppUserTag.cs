using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace movie_tracker_website.Models
{
    public class AppUserTag
    {
        [Required]
        public int TagId { get; set; }

        [Required]
        public string UserId { get; set; }

        [DefaultValue(0)]
        public int ValueImportance { get; set; }
    }
}