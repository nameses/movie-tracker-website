using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IMoviePageService
    {
        public Task<MoviePageViewModel> GetRandomPageAsync(AppUser user, ISession session);

        public Task<MoviePageViewModel> GetMoviePageAsync(int id, ISession session, AppUser user);
    }
}