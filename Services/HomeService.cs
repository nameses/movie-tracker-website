using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Services
{
    public class HomeService : IHomeService
    {
        private readonly ILogger<HomeService> _logger;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;

        public HomeService(ILogger<HomeService> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IMoviePageService moviePageService,
                IMovieService movieService,
                IMovieSessionListService movieSessionListService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _moviePageService = moviePageService;
            _movieService = movieService;
            _movieSessionListService = movieSessionListService;
        }

        public async Task<HomeViewModel> GetHomeViewModel(AppUser user, ISession session, int moviesCountInList)
        {
            var tasks = user.RelatedMovies
                 .FindAll(m => m.IfWatched)
                 .OrderBy(m => m.TimeWatched)
                 .Reverse()
                 .Take(8)
                 .Select(async m => await _movieService.GetReducedMovieAsync(m.ApiId))
                 .ToList();

            var watchedMovies = (await Task.WhenAll(tasks)).ToList();

            if (watchedMovies.Count > 8)
                watchedMovies.RemoveRange(moviesCountInList, watchedMovies.Count);

            //proccess list of recently viewed movies in session
            var viewedMovies = await _movieSessionListService.ShowMoviesListAsync(user, session);

            return new HomeViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                WatchedMovies = watchedMovies,
                ViewedMovies = viewedMovies
            };
        }
    }
}