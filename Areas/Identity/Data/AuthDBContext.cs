using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Models;

namespace movie_tracker_website.Data;

public class AuthDBContext : IdentityDbContext<AppUser>
{
    public AuthDBContext(DbContextOptions<AuthDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Movie model
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>()
            .HasMany(e => e.RelatedMovies)
            .WithOne(e => e.AppUser)
            .HasForeignKey(e => e.AppUserId);
        modelBuilder.Entity<Movie>()
            .HasOne(e => e.AppUser)
            .WithMany(e => e.RelatedMovies)
            .HasForeignKey(e => e.AppUserId)
            .IsRequired();

        //UserStatistic model
        modelBuilder.Entity<AppUser>()
            .HasOne(e => e.UserStatistic)
            .WithOne(e => e.AppUser)
            .HasForeignKey<UserStatistic>(e => e.AppUserId)
            .IsRequired();
        modelBuilder.Entity<UserStatistic>()
            .HasOne(e => e.AppUser)
            .WithOne(e => e.UserStatistic)
            .HasForeignKey<UserStatistic>(e => e.AppUserId)
            .IsRequired();

        //Follower model
        modelBuilder.Entity<AppUser>()
            .HasMany(e => e.Followers)
            .WithOne(e => e.FollowerUser)
            .HasForeignKey(e => e.FollowerUserId)
            .IsRequired();
        modelBuilder.Entity<Follower>()
            .HasOne(e => e.FollowerUser)
            .WithMany(e => e.Followers)
            .HasForeignKey(e => e.FollowerUserId)
            .IsRequired();

        modelBuilder.Entity<AppUser>()
            .HasMany(e => e.Followings)
            .WithOne(e => e.FollowingUser)
            .HasForeignKey(e => e.FollowingUserId)
            .IsRequired();
        modelBuilder.Entity<Follower>()
            .HasOne(e => e.FollowingUser)
            .WithMany(e => e.Followings)
            .HasForeignKey(e => e.FollowingUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        //Tag for each User
        modelBuilder.Entity<AppUser>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Users);

        //n:n relationship
        modelBuilder.Entity<AppUser>()
        .HasMany(e => e.Tags)
        .WithMany(e => e.Users)
        .UsingEntity<AppUserTag>(
            l => l.HasOne<Tag>().WithMany().HasForeignKey(e => e.TagId),
            r => r.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId));
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<UserStatistic> UserStatistics { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<AppUserTag> AppUserTags { get; set; }
}