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
    public class ProfileService : IProfileService
    {
        private const int FilmsCount = 4;
        private readonly ILogger<MoviePageController> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IStatisticService _statisticService;
        private readonly ITagService _tagService;

        public ProfileService(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IStatisticService statisticService,
                ITagService tagService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
            _movieService = movieService;
            _statisticService = statisticService;
            _tagService = tagService;
        }

        public async Task<ProfileViewModel> GetProfileAsync(AppUser user)
        {
            var followings = user.Followings
                .Select(f => AppUserViewModel.ConvertToReducedViewModel(
                        _context.Users.FirstOrDefault(u => u.Id == f.FollowingUserId)))
                .ToList();

            var tasks = user.RelatedMovies
                .Where(movie => movie.IfWatched && movie.IfFavourite)
                .OrderByDescending(movie => movie.TimeWatched)
                .Take(FilmsCount)
                .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                .ToList();
            var favMovies = (await Task.WhenAll(tasks)).ToList();
            tasks = user.RelatedMovies
                .Where(movie => movie.IfWatched)
                .OrderByDescending(movie => movie.TimeWatched)
                .Take(FilmsCount)
                .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                .ToList();
            var recentMovies = (await Task.WhenAll(tasks)).ToList();

            return new ProfileViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                FavouriteMovies = favMovies,
                RecentMovies = recentMovies,
                Followings = followings,
                Statistic = _statisticService.GetUserStatistic(user),
                Tags = _tagService.GetImportantTags(user, 10)
            };
        }

        public async Task<ProfileViewModel> GetProfileByUsernameAsync(AppUser currentUser, string username)
        {
            var user = _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .Include(u => u.Followings)
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper())
                .Result;

            var followings = user.Followings
                .Select(f => AppUserViewModel.ConvertToReducedViewModel(
                        _context.Users.FirstOrDefault(u => u.Id == f.FollowingUserId)))
                .ToList();

            var tasks = user.RelatedMovies
                .Where(movie => movie.IfWatched && movie.IfFavourite)// && movie.Rating == 5)
                .OrderByDescending(movie => movie.TimeWatched)
                .Take(FilmsCount)
                .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                .ToList();
            var favMovies = (await Task.WhenAll(tasks)).ToList();
            tasks = user.RelatedMovies
                .Where(movie => movie.IfWatched)
                .OrderByDescending(movie => movie.TimeWatched)
                .Take(FilmsCount)
                .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                .ToList();
            var recentMovies = (await Task.WhenAll(tasks)).ToList();

            return new ProfileViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(currentUser),
                IsUserFollowed = currentUser.Followings.FirstOrDefault(u => u.FollowingUserId == user.Id) != null,
                UserProfile = AppUserViewModel.convertToViewModel(user),
                FavouriteMovies = favMovies,
                RecentMovies = recentMovies,
                Followings = followings,
                Statistic = _statisticService.GetUserStatistic(user),
                Tags = _tagService.GetImportantTags(user, 10)
            };
        }

        public void Follow(AppUser currentUser, string usernameToFollow)
        {
            var user = _context.Users
                .Include(u => u.RelatedMovies)
                .Include(u => u.UserStatistic)
                .Include(u => u.Followings)
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == usernameToFollow.ToUpper())
                .Result;
            var follower = new Models.Follower()
            {
                FollowerUserId = currentUser.Id,
                FollowingUserId = user.Id,
            };
            _context.Followers.Add(follower);
            _context.SaveChanges();
        }

        public void Unfollow(AppUser currentUser, string usernameToUnfollow)
        {
            var user = _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == usernameToUnfollow.ToUpper())
                .Result;

            var follower = _context.Followers.FirstOrDefault(f => f.FollowerUserId == currentUser.Id && f.FollowingUserId == user.Id);
            _context.Followers.Remove(follower);

            _context.SaveChanges();
        }
    }
}