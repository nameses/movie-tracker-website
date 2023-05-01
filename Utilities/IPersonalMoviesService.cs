using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IPersonalMoviesService
    {
        public List<MovieViewModel>? GetListMovieViewModelsByPage(int pageIndex, int MovieCountPerPage);

        public PersonalMoviesViewModel? GetWatchedMoviesViewModel(int pageIndex, int MovieCountPerPage, AppUser user);
    }
}