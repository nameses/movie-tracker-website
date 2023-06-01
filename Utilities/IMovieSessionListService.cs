using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Utilities
{
    public interface IMovieSessionListService
    {
        public Task<List<MovieViewModel>> ProcessMoviesListAsync(ISession session, int id);

        public Task<List<MovieViewModel>> ShowMoviesListAsync(ISession session);
    }
}