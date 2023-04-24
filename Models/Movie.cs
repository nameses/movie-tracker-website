using System.ComponentModel.DataAnnotations.Schema;

namespace movie_tracker_website.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ApiId { get; set; }
        public double? Rating { get; set; }
        public DateTime TimeWatched { get; set; }

    }
}