﻿using Microsoft.AspNetCore.Authorization;
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
        private const int MovieCountPerPage = 16;

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

        [HttpGet]
        [Route("PersonalMovies/Index")]
        public async Task<IActionResult> Index(int pageIndex)
        {
            return RedirectToAction("GetWatchedMovies", "PersonalMovies", new { pageIndex = pageIndex });
        }

        [HttpGet]
        [Route("PersonalMovies/GetWatchedMovies/{pageIndex=1}")]
        public async Task<IActionResult> GetWatchedMovies(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var res = CheckPages(user, pageIndex, "GetWatchedMovies");
            if (res != null) return res;

            var personalMoviesViewModel = await _personalMoviesService.GetWatchedMoviesAsync(pageIndex, MovieCountPerPage, user);

            return View("Index", personalMoviesViewModel);
        }

        [HttpGet]
        [Route("PersonalMovies/GetFavouriteMovies/{pageIndex=1}")]
        public async Task<IActionResult> GetFavouriteMovies(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var res = CheckPages(user, pageIndex, "GetFavouriteMovies");
            if (res != null) return res;

            var personalMoviesViewModel = await _personalMoviesService.GetFavouriteMoviesAsync(pageIndex, MovieCountPerPage, user);

            return View("Index", personalMoviesViewModel);
        }

        [HttpGet]
        [Route("PersonalMovies/GetToWatchMovies/{pageIndex=1}")]
        public async Task<IActionResult> GetToWatchMovies(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var res = CheckPages(user, pageIndex, "GetToWatchMovies");
            if (res != null) return res;

            var personalMoviesViewModel = await _personalMoviesService.GetToWatchMoviesAsync(pageIndex, MovieCountPerPage, user);

            return View("Index", personalMoviesViewModel);
        }

        private RedirectToActionResult CheckPages(AppUser user, int pageIndex, string actionMethod)
        {
            if (pageIndex < 1) return RedirectToAction(actionMethod, "PersonalMovies", new { pageIndex = 1 });

            int allMoviesCount = user.RelatedMovies.FindAll(m => m.IfWatched).Count();
            int totalPages = allMoviesCount / MovieCountPerPage;

            if ((allMoviesCount - totalPages * MovieCountPerPage) % MovieCountPerPage > 0)
                totalPages++;

            if (totalPages != 0 && pageIndex > totalPages)
                return RedirectToAction(actionMethod, "PersonalMovies", new { pageIndex = totalPages });

            return null;
        }
    }
}