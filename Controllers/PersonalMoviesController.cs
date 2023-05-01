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
    public class PersonalMoviesController : Controller
    {
        private const int PagesCountPerPage = 8;

        private readonly ILogger<PersonalMoviesController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly IMoviePageService _moviePageService;

        public PersonalMoviesController(ILogger<PersonalMoviesController> logger,
                UserManager<AppUser> userManager,
                AuthDBContext context,
                IWebHostEnvironment webHostEnvironment,
                IConfiguration config,
                IMoviesList moviesList,
                IMoviePageService moviePageService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
            _moviesList = moviesList;
            _moviePageService = moviePageService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.RelatedMovies)
                .FirstOrDefaultAsync(u => u.Id == userId);
            //get watched movies and totalPages by user
            List<Models.Movie> allMovies = user.RelatedMovies
                .FindAll(m => m.IfWatched);
            int totalPages = allMovies.Count / PagesCountPerPage;
            if (allMovies.Count % PagesCountPerPage > 0) totalPages++;
            //check input pageIndex
            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > totalPages) pageIndex = totalPages;
            //get PagesCountPerPage movies
            var movies = allMovies.Skip((pageIndex - 1) * PagesCountPerPage)
            .Take(PagesCountPerPage)
            .OrderBy(m => m.TimeWatched)
            .Reverse()
            .Select(m => _moviePageService.GetReducedMovieById(m.ApiId))
            .ToList();
            //for correct viewing
            if (movies.Count != 0 && movies.Count < 8)
                for (int i = movies.Count; i <= 8; i++)
                    movies.Add(new MovieViewModel() { Id = -1 });

            var personalMoviesViewModel = new PersonalMoviesViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movies = movies,
                PageName = "Index",
                CurrentPage = pageIndex,
                TotalPages = totalPages
            };
            return View(personalMoviesViewModel);
        }
    }
}