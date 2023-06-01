using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Controllers;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ILogger<CommunityService> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;
        private readonly IProfileService _profileService;

        public CommunityService(ILogger<CommunityService> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService,
                IProfileService profileService)
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
        }

        public async Task<CommunityViewModel> GetCommunityMembers(AppUser user, int page, int usersPerPage)
        {
            //total pages count
            int usersCount = _context.Users.Count();
            var totalPages = usersCount / usersPerPage;
            if ((usersCount - totalPages * usersPerPage) % usersPerPage > 0)
                totalPages++;
            //pages correction
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var users = _context.Users
                .Include(u => u.UserStatistic)
                .OrderByDescending(u => u.UserStatistic.WatchedAmount)
                .Skip(usersPerPage * (page - 1))
                .Take(usersPerPage)
                .Select(u=>AppUserViewModel.ConvertToReducedStatsViewModel(user,u))
                .ToList();

            return new CommunityViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Users = users,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }
    }
}