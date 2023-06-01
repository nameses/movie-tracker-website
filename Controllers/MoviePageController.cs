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
using TMDbLib.Objects.Exceptions;
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
        private readonly IMoviePageService _moviePageService;

        public MoviePageController(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IMoviePageService moviePageService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _moviePageService = moviePageService;
        }

        [HttpGet]
        [Route("MoviePage/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var moviePageViewModel = await _moviePageService.GetMoviePageAsync(id, HttpContext.Session, user);
            return View(moviePageViewModel);
        }

        [HttpGet]
        [Route("MoviePage/random")]
        public async Task<IActionResult> RandomMovie()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var moviePageViewModel = await _moviePageService.GetRandomPageAsync(user, HttpContext.Session);

            return View("Index", moviePageViewModel);
        }

        [HttpPost]
        [Route("MoviePage/WatchedEntry/{id}")]
        public async Task<IActionResult> WatchedEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .FirstOrDefaultAsync(u => u.Id == userId);

            bool res = await _moviePageService.ChangeMovieWatchedStatus(user, movieEntry.ApiId);

            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        [HttpPost]
        [Route("MoviePage/FavouriteEntry/{id}")]
        public async Task<IActionResult> FavouriteEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .FirstOrDefaultAsync(u => u.Id == userId);

            bool res = await _moviePageService.ChangeMovieFavouriteStatus(user, movieEntry.ApiId);

            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        [HttpPost]
        [Route("MoviePage/ToWatchEntry/{id}")]
        public async Task<IActionResult> ToWatchEntry(MovieEntry movieEntry)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .FirstOrDefaultAsync(u => u.Id == userId);

            bool res = await _moviePageService.ChangeMovieToWatchStatus(user, movieEntry.ApiId);

            return RedirectToAction("Index", "MoviePage", new { id = movieEntry.ApiId });
        }

        public record class MovieEntry(int ApiId, double? Rating);
    }
}