using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Utilities
{
    public interface IMovieSessionListService
    {
        public List<MovieViewModel> ProcessSessionViewedMovies(ISession session, int id);

        public List<MovieViewModel>? ShowSessionViewedMovies(ISession session);
    }
}