namespace movie_tracker_website.ViewModels.PagesViews
{
    public class MoviesViewModel
    {
        public AppUserViewModel? CurrentUser { get; set; }
        public List<MovieViewModel>? Popular { get; set; }
        public List<MovieViewModel>? WatchedByFriends { get; set; }
        public List<MovieViewModel>? Recommendations { get; set; }
    }
}