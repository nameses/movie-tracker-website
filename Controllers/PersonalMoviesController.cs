using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class PersonalMoviesController : Controller
    {
        private const int MovieCountPerPage = 8;

        private readonly ILogger<PersonalMoviesController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IPersonalMoviesService _personalMoviesService;

        public PersonalMoviesController(ILogger<PersonalMoviesController> logger,
                UserManager<AppUser> userManager,
                AuthDBContext context,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IPersonalMoviesService personalMoviesService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
            _personalMoviesService = personalMoviesService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            return RedirectToAction("GetWatchedMovies", "PersonalMovies", new { pageIndex = pageIndex });
        }

        [HttpGet]
        [Route("PersonalMovies/GetWatchedMovies/{pageIndex}")]
        public async Task<IActionResult> GetWatchedMovies(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);
            //redirection to correct vals
            //check if pageindex < 1
            if (pageIndex < 1) return RedirectToAction("GetWatchedMovies", "PersonalMovies", new { pageIndex = 1 });
            //check if pageindex > total pages
            int totalPages = user.RelatedMovies.Select(m => m.IfWatched).Count() / MovieCountPerPage;
            if (totalPages % MovieCountPerPage > 0) totalPages++;
            if (pageIndex > totalPages)
                return RedirectToAction("GetWatchedMovies", "PersonalMovies", new { pageIndex = totalPages });

            var personalMoviesViewModel = _personalMoviesService.GetWatchedMoviesViewModel(pageIndex, MovieCountPerPage, user);

            return View("Index", personalMoviesViewModel);
        }

        [HttpGet]
        [Route("PersonalMovies/GetFavouriteMovies/{pageIndex}")]
        public async Task<IActionResult> GetFavouriteMovies(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);
            //redirection to correct vals
            //check if pageindex < 1
            if (pageIndex < 1) return RedirectToAction("GetFavouriteMovies", "PersonalMovies", new { pageIndex = 1 });
            //check if pageindex > total pages
            int totalPages = user.RelatedMovies.Select(m => m.IfFavourite).Count() / MovieCountPerPage;
            if (totalPages % MovieCountPerPage > 0) totalPages++;
            if (pageIndex > totalPages)
                return RedirectToAction("GetFavouriteMovies", "PersonalMovies", new { pageIndex = totalPages });

            var personalMoviesViewModel = _personalMoviesService.GetFavouriteMoviesViewModel(pageIndex, MovieCountPerPage, user);

            return View("Index", personalMoviesViewModel);
        }

        [HttpGet]
        [Route("PersonalMovies/GetToWatchMovies/{pageIndex}")]
        public async Task<IActionResult> GetToWatchMovies(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);
            //redirection to correct vals
            //check if pageindex < 1
            if (pageIndex < 1) return RedirectToAction("GetToWatchMovies", "PersonalMovies", new { pageIndex = 1 });
            //check if pageindex > total pages
            int totalPages = user.RelatedMovies.Select(m => m.IfToWatch).Count() / MovieCountPerPage;
            if (totalPages % MovieCountPerPage > 0) totalPages++;
            if (pageIndex > totalPages)
                return RedirectToAction("GetToWatchMovies", "PersonalMovies", new { pageIndex = totalPages });

            var personalMoviesViewModel = _personalMoviesService.GetToWatchMoviesViewModel(pageIndex, MovieCountPerPage, user);

            return View("Index", personalMoviesViewModel);
        }
    }
}