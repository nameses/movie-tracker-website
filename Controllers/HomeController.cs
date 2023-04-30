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
        private readonly ILogger<HomeController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMoviePageService _moviePageService;

        public HomeController(ILogger<HomeController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IMoviePageService moviePageService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _moviePageService = moviePageService;
        }

        public async Task<IActionResult> Index()
        {
            //AppUser user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var watchedMovies = user.RelatedMovies
                .FindAll(m => m.IfWatched)
                //.OrderBy((m1, m2) => m1.TimeWatched.CompareTo(m2.TimeWatched));
                .OrderBy(m => m.TimeWatched)
                .Take(8)
                .Select(m => _moviePageService.GetReducedMovieById(m.ApiId))
                .ToList();

            if (watchedMovies.Count < 8)
                for(int i = watchedMovies.Count; i<=8; i++)
                    watchedMovies.Add(new MovieViewModel() { Id = -1 });

            //proccess list of recently viewed movies in session
            List<MovieViewModel>? viewedMovies = _moviePageService.ShowSessionViewedMovies(HttpContext.Session);

            var homeViewModel = new HomeViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                WatchedMovies = watchedMovies,
                ViewedMovies = viewedMovies
            };
            return View(homeViewModel);
        }
    }
}