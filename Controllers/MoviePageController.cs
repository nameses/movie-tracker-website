using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Services.common;
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
        private const string SessionViewedMoviesName = "viewvedMovies";

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
                .Include(u => u.FavouriteMovies)
                .Include(u => u.MarkedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var movie = _moviePageService.GetMovieById(id);
            if (movie == null) return NotFound();

            movie.IfWatched = user.WatchedMovies.Any(m => m.ApiId == id);
            movie.IfFavourite = user.FavouriteMovies.Any(m => m.ApiId == id);
            movie.IfToWatch = user.MarkedMovies.Any(m => m.ApiId == id);

            List<MovieViewModel> similarMovies = _moviePageService.GetSimilarMovies(id);

            //checks if session is empty or gets a session's elements

            List<MovieViewModel>? viewedMovies = _moviePageService.ProccessSessionViewedMovies(HttpContext.Session, id);

            //view models prepearing
            var moviePageViewModel = new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies,
                ViewedMovies = viewedMovies
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

            movie.IfWatched = user.WatchedMovies != null && user.WatchedMovies.Any(m => m.ApiId == movie.Id);
            movie.IfFavourite = user.FavouriteMovies != null && user.FavouriteMovies.Any(m => m.ApiId == movie.Id);
            movie.IfToWatch = user.MarkedMovies != null && user.MarkedMovies.Any(m => m.ApiId == movie.Id);
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

            //add movie to watched list or remove
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
        public async Task<IActionResult> FavouriteEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.FavouriteMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            //add movie to watched list or remove
            if (user.FavouriteMovies.Any(m => m.ApiId == movieEntry.ApiId) == false)
            {
                user.FavouriteMovies.Add(new Models.Movie()
                {
                    ApiId = movieEntry.ApiId,
                    TimeWatched = DateTime.Now,
                });
            }
            else
            {
                var movie = user.FavouriteMovies.Find(m => m.ApiId == movieEntry.ApiId);
                //user.WatchedMovies.Remove(movie);
                _context.Movies.Remove(movie);
            }

            var res = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        [HttpPost]
        public async Task<IActionResult> ToWatchEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.MarkedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            //add movie to watched list or remove
            if (user.MarkedMovies.Any(m => m.ApiId == movieEntry.ApiId) == false)
            {
                user.MarkedMovies.Add(new Models.Movie()
                {
                    ApiId = movieEntry.ApiId,
                    TimeWatched = DateTime.Now,
                });
            }
            else
            {
                var movie = user.MarkedMovies.Find(m => m.ApiId == movieEntry.ApiId);
                //user.WatchedMovies.Remove(movie);
                _context.Movies.Remove(movie);
            }

            var res = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        public record class MovieEntry(int ApiId, double? Rating);
    }
}