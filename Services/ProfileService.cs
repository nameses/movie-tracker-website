using Microsoft.AspNetCore.Identity;
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

        public ProfileViewModel GetProfileViewModel(AppUser user)
        {
            int FilmsCount = 4;
            var favMovies = user.RelatedMovies
                .Where(movie => movie.IfWatched && movie.IfFavourite)// && movie.Rating == 5)
                .OrderByDescending(movie => movie.TimeWatched)
                .Take(FilmsCount)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();
            var recentMovies = user.RelatedMovies
                .Where(movie => movie.IfWatched)
                .OrderByDescending(movie => movie.TimeWatched)
                .Take(FilmsCount)
                .Select(m => _movieService.GetReducedMovieById(m.ApiId))
                .ToList();

            for (int i = favMovies.Count; i < FilmsCount; i++)
                favMovies.Add(new MovieViewModel() { Id = -1 });

            for (int i = recentMovies.Count; i < FilmsCount; i++)
                recentMovies.Add(new MovieViewModel() { Id = -1 });
            //if (recentMovies.Count > 0) { }

            return new ProfileViewModel
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                FavouriteMovies = favMovies,
                RecentMovies = recentMovies,
                Statistic = _statisticService.GetUserStatistic(user),
                Tags = _tagService.GetImportantTags(user, 10)
            };
        }
    }
}