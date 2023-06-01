using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services.common;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Services
{
    public class PersonalMoviesService : IPersonalMoviesService
    {
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMovieService _movieService;

        public PersonalMoviesService(IConfiguration config,
            IMoviesList moviesList,
            AuthDBContext context,
            UserManager<AppUser> userManager,
            IMovieService movieService)
        {
            _context = context;
            _userManager = userManager;
            _movieService = movieService;
            _config = config;
            _moviesList = moviesList;
        }

        public async Task<PersonalMoviesViewModel> GetWatchedMoviesAsync(int pageIndex, int MovieCountPerPage, AppUser user)
        {
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfWatched);
            int totalPages = allMovies.Count / MovieCountPerPage;
            if ((allMovies.Count - totalPages * MovieCountPerPage) % MovieCountPerPage > 0)
                totalPages++;

            return new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Movies = await GetMoviesListAsync(allMovies, pageIndex, MovieCountPerPage),
                PageName = "GetWatchedMovies",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }

        public async Task<PersonalMoviesViewModel> GetFavouriteMoviesAsync(int pageIndex, int MovieCountPerPage, AppUser user)
        {
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfFavourite);
            //correct totalPages amount
            int totalPages = allMovies.Count / MovieCountPerPage;
            if ((allMovies.Count - totalPages * MovieCountPerPage) % MovieCountPerPage > 0)
                totalPages++;

            return new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Movies = await GetMoviesListAsync(allMovies, pageIndex, MovieCountPerPage),
                PageName = "GetFavouriteMovies",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }

        public async Task<PersonalMoviesViewModel> GetToWatchMoviesAsync(int pageIndex, int MovieCountPerPage, AppUser user)
        {
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfToWatch);
            //correct totalPages amount
            int totalPages = allMovies.Count / MovieCountPerPage;
            if ((allMovies.Count - totalPages * MovieCountPerPage) % MovieCountPerPage > 0)
                totalPages++;

            return new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Movies = await GetMoviesListAsync(allMovies, pageIndex, MovieCountPerPage),
                PageName = "GetToWatchMovies",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }

        private async Task<List<MovieViewModel>> GetMoviesListAsync(List<Models.Movie> movies, int page, int moviesCountPerPage)
        {
            var tasks = movies.OrderBy(m => m.TimeWatched)
                .Skip((page - 1) * moviesCountPerPage)
                .Take(moviesCountPerPage)
                .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                .ToList();

            return (await Task.WhenAll(tasks)).ToList();
        }
    }
}