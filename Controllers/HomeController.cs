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

        public HomeController(ILogger<HomeController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _moviePageService = moviePageService;
            _movieService = movieService;
            _movieSessionListService = movieSessionListService;
        }

        public async Task<IActionResult> Index()
        {
            //AppUser user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var tasks = user.RelatedMovies
                .FindAll(m => m.IfWatched)
                .OrderBy(m => m.TimeWatched)
                .Reverse()
                .Take(8)
                .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                .ToList();

            var watchedMovies = (await Task.WhenAll(tasks)).ToList();

            if (watchedMovies.Count > 8)
                watchedMovies.RemoveRange(MoviesListValue, watchedMovies.Count);

            //proccess list of recently viewed movies in session
            var viewedMovies = await _movieSessionListService.ShowMoviesListAsync(user, HttpContext.Session);

            var homeViewModel = new HomeViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                WatchedMovies = watchedMovies,
                ViewedMovies = viewedMovies
            };
            return View(homeViewModel);
        }
    }
}