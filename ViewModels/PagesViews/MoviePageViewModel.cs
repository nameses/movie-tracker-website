namespace movie_tracker_website.ViewModels.PagesViews
{
    public class MoviePageViewModel
    {
        public AppUserViewModel CurrentUser { get; set; }
        public MovieViewModel Movie { get; set; }
        public List<MovieViewModel> SimilarMovies { get; set; }
    }
}