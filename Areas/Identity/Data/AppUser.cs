using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Models;

namespace movie_tracker_website.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser
{
    public string? ImagePath { get; set; }

    public virtual List<Movie>? RelatedMovies { get; set; } = new List<Movie>();
}