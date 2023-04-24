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
        private readonly Data.AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;

        public MoviePageController(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
        }

        [HttpGet]
        [Route("MoviePage/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.WatchedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var movie = _moviePageService.GetMovieById(id);
            if (movie == null) return NotFound();

            movie.IfWatched = user.WatchedMovies.Any(m => m.ApiId == id);

            List<MovieViewModel> similarMovies = _moviePageService.GetSimilarMovies(id);

            var moviePageViewModel = new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies
            };
            return View(moviePageViewModel);
        }

        [HttpGet]
        [Route("MoviePage/random")]
        public async Task<IActionResult> RandomMovie()
        {
            AppUser user = _userManager.GetUserAsync(User).Result;

            var movie = _moviePageService.GetRandomMovie();
            if (movie == null) return NotFound();

            List<MovieViewModel> similarMovies = _moviePageService.GetSimilarMovies(movie.Id);

            var moviePageViewModel = new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies
            };

            return View("Index", moviePageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> WatchedEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.WatchedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user.WatchedMovies.Any(m => m.ApiId == movieEntry.ApiId) == false)
            {
                user.WatchedMovies.Add(new Models.Movie()
                {
                    ApiId = movieEntry.ApiId,
                    TimeWatched = DateTime.Now,
                });
            }
            else
            {
                var movie = user.WatchedMovies.Find(m => m.ApiId == movieEntry.ApiId);
                //user.WatchedMovies.Remove(movie);
                _context.Movies.Remove(movie);
            }
            var res = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        [HttpPost]
        public void FavouriteEntry()
        {
        }

        [HttpPost]
        public void ToWatchEntry()
        {
        }

        public record class MovieEntry(int ApiId, double? Rating);
    }
}