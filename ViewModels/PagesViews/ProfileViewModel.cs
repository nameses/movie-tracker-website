namespace movie_tracker_website.ViewModels.PagesViews
{
    public class ProfileViewModel
    {
        public AppUserViewModel CurrentUser { get; set; }
        public List<MovieViewModel>? FavouriteMovies { get; set; }
        public List<MovieViewModel>? RecentMovies { get; set; }
    }
}