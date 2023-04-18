using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IMoviePageService
    {
        public MovieViewModel? GetMovieById(int id);

        public MovieViewModel GetRandomMovie();
    }
}