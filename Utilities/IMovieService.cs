using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Utilities
{
    public interface IMovieService
    {
        public Task<MovieViewModel> GetMovieAsync(int id);

        public Task<MovieViewModel> GetReducedMovieAsync(int id);
    }
}