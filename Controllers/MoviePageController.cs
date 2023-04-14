using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class MoviePageController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;

        public MoviePageController(ILogger<HomeController> logger,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config)
        {
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
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

            //TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]);
            //Movie movie = client.GetMovieAsync(47964).Result;

            return View(appUser);
        }
    }
}