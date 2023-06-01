using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Utilities
{
    public interface IMovieSessionListService
    {
        public Task<List<MovieViewModel>> ProcessMoviesListAsync(AppUser user, ISession session, int id);

        public Task<List<MovieViewModel>> ShowMoviesListAsync(AppUser user, ISession session);
    }
}