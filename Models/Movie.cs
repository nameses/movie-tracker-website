using Microsoft.Build.Framework;
using movie_tracker_website.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_tracker_website.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Microsoft.Build.Framework.Required]
        public int ApiId { get; set; }

        [DefaultValue(false)]
        public bool IfWatched { get; set; }

        [DefaultValue(false)]
        public bool IfFavourite { get; set; }

        [DefaultValue(false)]
        public bool IfToWatch { get; set; }

        [Range(1, 10)]
        public int? Rating { get; set; }

        public DateTime TimeWatched { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
    }
}