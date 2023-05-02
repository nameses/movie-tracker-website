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

        public List<MovieViewModel>? GetListMovieViewModelsByPage(int pageIndex, int MovieCountPerPage)
        {
            return null;
        }

        public PersonalMoviesViewModel? GetWatchedMoviesViewModel(int pageIndex, int MovieCountPerPage, AppUser user)
        {
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfWatched);
            int totalPages = allMovies.Count / MovieCountPerPage;
            if (allMovies.Count % MovieCountPerPage > 0) totalPages++;
            //get PagesCountPerPage movies
            var movies = allMovies.OrderBy(m => m.TimeWatched)
                .Skip((pageIndex - 1) * MovieCountPerPage)
                .Take(MovieCountPerPage)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();
            //for correct viewing
            if (movies.Count != 0 && movies.Count < 8)
                for (int i = movies.Count; i <= 8; i++)
                    movies.Add(new MovieViewModel() { Id = -1 });

            return new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movies = movies,
                PageName = "GetWatchedMovies",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }

        public PersonalMoviesViewModel? GetFavouriteMoviesViewModel(int pageIndex, int MovieCountPerPage, AppUser user)
        {
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfFavourite);
            int totalPages = allMovies.Count / MovieCountPerPage;
            if (allMovies.Count % MovieCountPerPage > 0) totalPages++;
            //get PagesCountPerPage movies
            var movies = allMovies.OrderBy(m => m.TimeWatched)
                .Skip((pageIndex - 1) * MovieCountPerPage)
                .Take(MovieCountPerPage)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();
            //for correct viewing
            if (movies.Count != 0 && movies.Count < 8)
                for (int i = movies.Count; i <= 8; i++)
                    movies.Add(new MovieViewModel() { Id = -1 });

            return new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movies = movies,
                PageName = "GetFavouriteMovies",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }

        public PersonalMoviesViewModel? GetToWatchMoviesViewModel(int pageIndex, int MovieCountPerPage, AppUser user)
        {
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfToWatch);
            int totalPages = allMovies.Count / MovieCountPerPage;
            if (allMovies.Count % MovieCountPerPage > 0) totalPages++;
            //get PagesCountPerPage movies
            var movies = allMovies.OrderBy(m => m.TimeWatched)
                .Skip((pageIndex - 1) * MovieCountPerPage)
                .Take(MovieCountPerPage)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();
            //for correct viewing
            if (movies.Count != 0 && movies.Count < 8)
                for (int i = movies.Count; i <= 8; i++)
                    movies.Add(new MovieViewModel() { Id = -1 });

            return new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movies = movies,
                PageName = "GetToWatchMovies",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
        }
    }
}