using System.Collections.Generic;

namespace movie_tracker_website.ViewModels.PagesViews
{
    public class HomeViewModel
    {
        public AppUserViewModel? CurrentUser { get; set; }
        public List<MovieViewModel>? WatchedMovies { get; set; } = new List<MovieViewModel>();
        public List<MovieViewModel>? ViewedMovies { get; set; } = new List<MovieViewModel>();
    }
}