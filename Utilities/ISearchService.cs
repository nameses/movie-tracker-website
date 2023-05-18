using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface ISearchService
    {
        public async Task<SearchViewModel> GetMoviesByQuery(string query);
    }
}