using movie_tracker_website.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_tracker_website.Models
{
    public class UserStatistic
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DefaultValue(0)]
        public int WatchedAmount { get; set; }

        [DefaultValue(0)]
        public int FavouriteAmount { get; set; }

        [DefaultValue(0)]
        public int ToWatchAmount { get; set; }

        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
    }
}