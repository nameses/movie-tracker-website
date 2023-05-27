using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Services.common;
using movie_tracker_website.Utilities;
using static movie_tracker_website.Controllers.MoviePageController;

namespace movie_tracker_website.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<MoviePageController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;
        private readonly IProfileService _profileService;

        public ProfileController(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService,
                IProfileService profileService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
            _movieService = movieService;
            _movieSessionListService = movieSessionListService;
            _profileService = profileService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var profileViewModel = _profileService.GetProfileViewModel(user);

            return View(profileViewModel);
        }

        [HttpGet]
        [Route("Profile/{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .Include(u => u.Followings)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            //if current user == searching profile
            if (currentUser.NormalizedUserName == username.ToUpper())
                return RedirectToAction("Index", "Profile");

            var profileViewModel = _profileService.GetProfileById(currentUser, username);

            return View("UserProfile", profileViewModel);
        }

        [HttpPost]
        [Route("Profile/Follow/{username}")]
        public async Task<IActionResult> Follow(string username)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _context.Users
                .Include(u => u.Followings)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            _profileService.Follow(currentUser, username);

            return RedirectToAction("GetProfile", "Profile", new { username = username });
        }

        [HttpPost]
        [Route("Profile/Unfollow/{username}")]
        public async Task<IActionResult> Unfollow(string username)
        {
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = await _context.Users
                .Include(u => u.Followings)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            _profileService.Unfollow(currentUser, username);

            return RedirectToAction("GetProfile", "Profile", new { username = username });
        }
    }
}