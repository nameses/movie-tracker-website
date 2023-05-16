using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Controllers;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Services.common
{
    public class StatisticService : IStatisticService
    {
        private readonly IConfiguration _config;
        private readonly AuthDBContext _context;
        private readonly ILogger<MoviePageController> _logger;
        private readonly IMoviePageService _moviePageService;
        private readonly IMovieService _movieService;
        private readonly IMoviesList _moviesList;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StatisticService(ILogger<MoviePageController> logger,
                AuthDBContext context,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService,
                IMovieService movieService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
            _movieService = movieService;
        }

        public StatisticViewModel GetUserStatistic(AppUser user)
        {
            return new StatisticViewModel()
            {
                StatisticDict = new Dictionary<string, int>()
                {
                    { "переглянуто", user.RelatedMovies.Where(m => m.IfWatched).Count() },
                    { "цього року", user.RelatedMovies
                        .Where(m => m.IfWatched)
                        .Where(m => m.TimeWatched.Year==DateTime.Now.Year)
                        .Count()
                    },
                    { "улюблених", user.RelatedMovies.Where(m => m.IfFavourite).Count()},
                    { "до перегляду", user.RelatedMovies.Where(m => m.IfToWatch).Count()}
                }
            };
        }
    }
}