using Microsoft.Build.Framework;
using movie_tracker_website.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_tracker_website.Models
{
    public class Follower
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }

        [Required]
        public string? FollowerUserId { get; set; }

        [Required]
        public string? FollowingUserId { get; set; }

        [Required]
        public virtual AppUser? FollowerUser { get; set; }

        [Required]
        public virtual AppUser? FollowingUser { get; set; }
    }
}