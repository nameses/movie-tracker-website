using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_tracker_website.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ApiId { get; set; }

        [DefaultValue(false)]
        public bool IfWatched { get; set; }

        [DefaultValue(false)]
        public bool IfFavourite { get; set; }

        [DefaultValue(false)]
        public bool IfToWatch { get; set; }

        public double? Rating { get; set; }
        public DateTime TimeWatched { get; set; }
    }
}