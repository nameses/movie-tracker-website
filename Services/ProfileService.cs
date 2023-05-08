using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Controllers;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ILogger<MoviePageController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;

        public ProfileService(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
            _movieService = movieService;
        }

        public ProfileViewModel GetProfileViewModel(AppUser user)
        {
            var favMovies = user.RelatedMovies
                .Where(movie => movie.IfWatched && movie.IfFavourite)// && movie.Rating == 5)
                .OrderBy(movie => movie.TimeWatched)
                .Take(4)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();
            var recentMovies = user.RelatedMovies
                .Where(movie => movie.IfWatched && movie.IfFavourite)// && movie.Rating == 5)
                .OrderBy(movie => movie.TimeWatched)
                .Take(4)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();

            return new ProfileViewModel
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                FavouriteMovies = favMovies,
                RecentMovies = recentMovies
            };
        }
    }
}