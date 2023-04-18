using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Languages;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class MoviePageController : Controller
    {
        private readonly ILogger<MoviePageController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;

        public MoviePageController(ILogger<MoviePageController> logger,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService)
        {
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
        }

        [Route("MoviePage/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            AppUser user = await _userManager.GetUserAsync(User);

            MovieViewModel movie = _moviePageService.GetMovieById(id);

            if (movie == null) return NotFound();

            var moviePageViewModel = new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = movie
            };
            return View(moviePageViewModel);
        }

        [Route("MoviePage/random")]
        public async Task<IActionResult> RandomMovie()
        {
            AppUser user = _userManager.GetUserAsync(User).Result;

            var moviePageViewModel = new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = _moviePageService.GetRandomMovie(),
            };

            return View("Index", moviePageViewModel);
        }
    }
}