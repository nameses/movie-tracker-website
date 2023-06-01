using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IPersonalMoviesService
    {
        public Task<PersonalMoviesViewModel> GetWatchedMoviesAsync(int pageIndex, int MovieCountPerPage, AppUser user);

        public Task<PersonalMoviesViewModel> GetFavouriteMoviesAsync(int pageIndex, int MovieCountPerPage, AppUser user);

        public Task<PersonalMoviesViewModel> GetToWatchMoviesAsync(int pageIndex, int MovieCountPerPage, AppUser user);
    }
}