using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IMoviePageService
    {
        public MoviePageViewModel GetMoviePageViewModel(int id, ISession session, AppUser user);

        public List<MovieViewModel> GetSimilarMovies(int id);

        public MovieViewModel GetRandomMovie();
    }
}