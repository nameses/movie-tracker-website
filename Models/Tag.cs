using movie_tracker_website.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_tracker_website.Models
{
    public class Tag
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public List<AppUser> Users { get; set; } = new List<AppUser> { };
    }
}