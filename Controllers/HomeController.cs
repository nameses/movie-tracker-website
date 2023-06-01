using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private const int MoviesListValue = 8;

        private readonly ILogger<HomeController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;
        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService,
                IHomeService homeService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _moviePageService = moviePageService;
            _movieService = movieService;
            _movieSessionListService = movieSessionListService;
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            //AppUser user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var homeViewModel = await _homeService.GetHomeViewModel(user, HttpContext.Session, MoviesListValue);

            return View(homeViewModel);
        }
    }
}