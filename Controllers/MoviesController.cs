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
    public class MoviesController : Controller
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMoviesService _moviesService;

        public MoviesController(ILogger<MoviesController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IMoviesService moviesService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _moviesService = moviesService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.Followings)
                .Include(u => u.Tags)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var moviesViewModel = await _moviesService.GetMainInfo(user);
            return View(moviesViewModel);
        }
    }
}