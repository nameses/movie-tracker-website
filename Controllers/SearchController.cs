﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Services.common;
using movie_tracker_website.Utilities;

namespace movie_tracker_website.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;
        private readonly IProfileService _profileService;
        private readonly ISearchService _searchService;

        public SearchController(ILogger<SearchController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService,
                IProfileService profileService,
                ISearchService searchService)
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
            _searchService = searchService;
        }

        [Route("Search/Index")]
        public async Task<IActionResult> Index(string query)
        {
            return RedirectToAction("GetMoviesByQuery", "Search", new { query = query });
        }

        [Route("Search/GetMoviesByQuery")]
        public async Task<IActionResult> GetMoviesByQuery(string query, int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                //.Include(u => u.RelatedMovies)
                //.Include(u => u.UserStatistic)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var searchViewModel = await _searchService.GetMoviesByQuery(user, query, pageIndex);

            return View("Index", searchViewModel);
        }
    }
}