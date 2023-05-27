using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace movie_tracker_website.ViewModels.PagesViews
{
    public class ProfileViewModel
    {
        public AppUserViewModel? CurrentUser { get; set; }
        public bool? IsUserFollowed { get; set; }
        public AppUserViewModel? UserProfile { get; set; }
        public List<MovieViewModel?>? FavouriteMovies { get; set; }
        public List<MovieViewModel?>? RecentMovies { get; set; }
        public StatisticViewModel? Statistic { get; set; }
        public List<string?>? Tags { get; set; }
    }
}