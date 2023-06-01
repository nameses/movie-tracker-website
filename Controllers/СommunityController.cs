using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class CommunityController : Controller
    {
        private const int UsersPerPage = 10;

        private readonly ILogger<CommunityController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;
        private readonly IProfileService _profileService;
        private readonly ICommunityService _communityService;

        public CommunityController(ILogger<CommunityController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService,
                IProfileService profileService,
                ICommunityService communityService)
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
            _communityService = communityService;
        }

        [HttpGet]
        [ActionName("Index")]
        [Route("Community/Index")]
        public IActionResult Index()
        {
            return RedirectToAction("GetCommunityMembers", "Community", new { pageIndex = 1 });
        }

        [HttpGet]
        [ActionName("GetCommunityMembers")]
        [Route("Community/GetCommunityMembers/{pageIndex=1}")]
        public async Task<IActionResult> GetCommunityMembers(int pageIndex)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u=>u.Followers)
                .Include(u => u.Followings)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var communityViewModel = await _communityService.GetCommunityMembers(user, pageIndex, UsersPerPage);

            return View("Index", communityViewModel);
        }

        [HttpPost]
        [ActionName("Follow")]
        [Route("Community/Follow/{username}")]
        public async Task<IActionResult> Follow(string username, int page)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            _profileService.Follow(user, username);

            return RedirectToAction("GetCommunityMembers", "Community", new { pageIndex = page }); ;
        }

        [HttpPost]
        [ActionName("Unfollow")]
        [Route("Community/Unfollow/{username}")]
        public async Task<IActionResult> Unfollow(string username, int page)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            _profileService.Unfollow(user, username);

            return RedirectToAction("GetCommunityMembers", "Community", new { pageIndex = page }); ;
        }
    }
}