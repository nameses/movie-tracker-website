using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IMoviePageService
    {
        public List<MovieViewModel> GetSimilarMovies(int id);

        public MovieViewModel? GetMovieById(int id);

        public MovieViewModel GetRandomMovie();

        public List<MovieViewModel> ProcessSessionViewedMovies(ISession session, int id);
    }
}