using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Models;
using System.ComponentModel.DataAnnotations;

namespace movie_tracker_website.ViewModels
{
    public class AppUserViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string? ImagePath { get; set; }

        public UserStatisticViewModel? Statistic { get; set; }

        public static AppUserViewModel ConvertToViewModel(AppUser user)
        {
            return new AppUserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                ImagePath = user.ImagePath
            };
        }

        public static AppUserViewModel ConvertToReducedViewModel(AppUser user)
        {
            return new AppUserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                ImagePath = user.ImagePath
            };
        }

        public static AppUserViewModel ConvertToReducedStatsViewModel(AppUser user)
        {
            return new AppUserViewModel()
            {
                Id = user.Id,
                Username = user.UserName,
                ImagePath = user.ImagePath,
                Statistic = new UserStatisticViewModel()
                {
                    WatchedAmount = user.UserStatistic.WatchedAmount,
                    FavouriteAmount = user.UserStatistic.FavouriteAmount,
                    ToWatchAmount = user.UserStatistic.ToWatchAmount
                }
            };
        }
    }
}