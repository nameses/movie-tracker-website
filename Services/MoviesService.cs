using Microsoft.AspNetCore.Mvc;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using movie_tracker_website.Data;
using NuGet.Packaging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace movie_tracker_website.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly AuthDBContext _context;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;

        public MoviesService(IConfiguration config,
            IMoviesList moviesList,
            AuthDBContext context,
            IMovieService movieService,
            IMovieSessionListService movieSessionListService)
        {
            _context = context;
            _movieService = movieService;
            _movieSessionListService = movieSessionListService;
            _config = config;
            _moviesList = moviesList;
        }

        public async Task<MoviesViewModel> GetMainInfo(AppUser user)
        {
            var popular = new List<MovieViewModel>();
            var watchedByFriends = new List<MovieViewModel>();
            var recommendations = new List<MovieViewModel>();

            using (var client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                //popular movies now (top 9)
                SearchContainer<SearchMovie> results = await client.GetMoviePopularListAsync();

                popular.AddRange(results.Results
                    .OrderByDescending(mov => mov.Popularity)
                    .Take(9)
                    .Where(mov => mov.PosterPath != null)
                    .Select(MovieViewModel.convertToReducedMovieViewModel));

                //watched by following users
                //take the last 10 movies, watched by following users
                List<string?> idsOfFollowingUsers = user.Followings
                    .Select(f => f.FollowingUserId)
                    .ToList();

                var tasks = _context.Movies
                    .Where(m => idsOfFollowingUsers.Contains(m.AppUserId))
                    .OrderByDescending(m => m.TimeWatched)
                    //get last 50 movies (to improve productivity)
                    .Take(50)
                    //distinct
                    .Distinct()
                    //take last 10
                    .Take(10)
                    .OrderBy(m => m.TimeWatched)
                    .Select(mov => _movieService.GetReducedMovieAsync(mov.ApiId))
                    .ToList();

                watchedByFriends = (await Task.WhenAll(tasks)).ToList();

                //recommendations
                //take top 10 tags for movies for current user
                List<int> idsOfTags = _context.AppUserTags
                    .Where(t => t.UserId == user.Id)
                    .OrderBy(t => t.ValueImportance)
                    .Take(10)
                    .Select(t => _context.Tags.First(trueTag => trueTag.Id == t.TagId).ApiId)
                    .ToList();
                //take 1 page of found films(with filtering) for each tag
                List<SearchMovie> foundMovies = new();
                foreach (var tag in idsOfTags)
                {
                    //find 1 page of movies by tag
                    var foundMoviesPerTag = await client.GetKeywordMoviesAsync(tag);
                    // apply filters and add to all
                    foundMovies.AddRange(foundMoviesPerTag.Results
                        .Where(r => r.PosterPath != null)
                    );
                }
                //list of already watched movies for filters
                var watched = user.RelatedMovies
                    .Where(m => m.IfWatched)
                    .Select(m => m.ApiId)
                    .ToList();
                //add to recs top 10 of all found movies
                recommendations.AddRange(foundMovies
                    .ToList()
                    .OrderByDescending(m => m.Popularity)
                    //distinct
                    .GroupBy(m => m.Id)
                    .Select(g => g.First())
                    //filter for watched movies
                    .Where(m => !watched.Contains(m.Id))
                    .Take(10)
                    .Select(MovieViewModel.convertToReducedMovieViewModel)
                );
            }
            return new MoviesViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Popular = popular,
                WatchedByFriends = watchedByFriends,
                Recommendations = recommendations
            };
        }
    }
}