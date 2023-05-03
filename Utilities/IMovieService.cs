using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Utilities
{
    public interface IMovieService
    {
        public MovieViewModel? GetMovieById(int id);

        public MovieViewModel? GetReducedMovieById(int id);
    }
}