using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IMoviePageService
    {
        public Task<MoviePageViewModel> GetRandomPageAsync(AppUser user, ISession session);

        public Task<MoviePageViewModel> GetMoviePageAsync(int id, ISession session, AppUser user);

        //posts
        public Task<bool> ChangeMovieWatchedStatus(AppUser user, int ApiId, double? Rating = null);

        public Task<bool> ChangeMovieFavouriteStatus(AppUser user, int ApiId, double? Rating = null);

        public Task<bool> ChangeMovieToWatchStatus(AppUser user, int ApiId, double? Rating = null);
    }
}