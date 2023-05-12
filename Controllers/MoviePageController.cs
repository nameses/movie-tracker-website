using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Models;
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
        private readonly ILogger<MoviePageController> _logger;
        private readonly Data.AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;

        public MoviePageController(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService)
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
        }

        [HttpGet]
        [Route("MoviePage/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var moviePageViewModel = _moviePageService.GetMoviePageViewModel(id, HttpContext.Session, user);
            return View(moviePageViewModel);
        }

        [HttpGet]
        [Route("MoviePage/random")]
        public async Task<IActionResult> RandomMovie()
        {
            //AppUser user = _userManager.GetUserAsync(User).Result;
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var movie = _moviePageService.GetRandomMovie();
            if (movie == null) return NotFound();

            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == movie.Id);
            if (movieFromDB != null)
            {
                movie.IfWatched = movieFromDB.IfWatched;
                movie.IfFavourite = movieFromDB.IfFavourite;
                movie.IfToWatch = movieFromDB.IfToWatch;
            }
            //find similar movies to current movie
            List<MovieViewModel> similarMovies = _moviePageService.GetSimilarMovies(movie.Id);
            //proccess list of recently viewed movies in session
            List<MovieViewModel>? viewedMovies = _movieSessionListService.ProcessSessionViewedMovies(HttpContext.Session, movie.Id);

            var moviePageViewModel = new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies,
                ViewedMovies = viewedMovies
            };

            return View("Index", moviePageViewModel);
        }

        [HttpPost]
        [Route("MoviePage/WatchedEntry/{id}")]
        public async Task<IActionResult> WatchedEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == movieEntry.ApiId);
            if (movieFromDB != null)
            {
                movieFromDB.IfWatched = !movieFromDB.IfWatched;
                //set new time if movie is corrected to watched
                if (movieFromDB.IfWatched)
                {
                    user.UserStatistic.WatchedAmount++;

                    movieFromDB.TimeWatched = DateTime.Now;
                }
            }
            //add new movie entry then
            else
            {
                user.RelatedMovies.Add(new Models.Movie()
                {
                    ApiId = movieEntry.ApiId,
                    IfWatched = true,
                    TimeWatched = DateTime.Now,
                });
            }
            var res = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        [HttpPost]
        [Route("MoviePage/FavouriteEntry/{id}")]
        public async Task<IActionResult> FavouriteEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == movieEntry.ApiId);
            if (movieFromDB != null)
            {
                movieFromDB.IfFavourite = !movieFromDB.IfFavourite;
            }
            //add new movie entry then
            else
            {
                user.RelatedMovies.Add(new Models.Movie()
                {
                    ApiId = movieEntry.ApiId,
                    IfFavourite = true,
                    TimeWatched = DateTime.Now,
                });
            }

            var res = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        [HttpPost]
        [Route("MoviePage/ToWatchEntry/{id}")]
        public async Task<IActionResult> ToWatchEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == movieEntry.ApiId);
            if (movieFromDB != null)
            {
                movieFromDB.IfToWatch = !movieFromDB.IfToWatch;
            }
            //add new movie entry then
            else
            {
                user.RelatedMovies.Add(new Models.Movie()
                {
                    ApiId = movieEntry.ApiId,
                    IfToWatch = true,
                    TimeWatched = DateTime.Now,
                });
            }

            var res = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        public record class MovieEntry(int ApiId, double? Rating);
    }
}