using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Models;

namespace movie_tracker_website.Data;

public class AuthDBContext : IdentityDbContext<AppUser>
{
    public AuthDBContext(DbContextOptions<AuthDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<Movie> Movies { get; set; }
}