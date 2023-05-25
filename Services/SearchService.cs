using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Controllers;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;
using NuGet.Packaging;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace movie_tracker_website.Services
{
    public class SearchService : ISearchService
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

        public SearchService(ILogger<SearchController> logger,
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

        public async Task<SearchViewModel> GetMoviesByQuery(AppUser user, string query)
        {
            var movies = new List<MovieViewModel>();
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                SearchContainer<SearchMovie> results = await client.SearchMovieAsync(query);
                movies.AddRange(results.Results
                    .Select(mov => client.GetMovieAsync(mov.Id,
                            MovieMethods.AlternativeTitles | MovieMethods.Credits).Result)
                    .Where(mov => mov.PosterPath != null)
                    .Select(MovieViewModel.convertToSearchMovieViewModel));
            }

            return new SearchViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movies = movies,
                Query = query
            };
        }
    }
}