using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using TMDbLib.Client;
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

        public MoviePageController(ILogger<MoviePageController> logger,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList)
        {
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var appUser = new AppUserViewModel()
            {
                Username = user.UserName,
                Email = user.Email,
                ImagePath = user.ImagePath
            };
            return View(appUser);
        }

        [Route("MoviePage/random")]
        public async Task<IActionResult> RandomMovie()
        {
            var user = await _userManager.GetUserAsync(User);

            var appUser = new AppUserViewModel()
            {
                Username = user.UserName,
                Email = user.Email,
                ImagePath = user.ImagePath
            };

            Console.WriteLine(_moviesList.GetRandomMovieID());

            return View("Index", appUser);
        }
    }
}