using Microsoft.IdentityModel.Tokens;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Services.common
{
    public class MovieSessionListService : IMovieSessionListService
    {
        private const string SessionListName = "viewedMovies";
        private const int ValueToStore = 8;

        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly AuthDBContext _context;
        private readonly IMovieService _movieService;

        public MovieSessionListService(IConfiguration config,
            IMoviesList moviesList,
            AuthDBContext context,
            IMovieService movieService)
        {
            _context = context;
            _movieService = movieService;
            _config = config;
            _moviesList = moviesList;
        }

        public async Task<List<MovieViewModel>> ProcessMoviesListAsync(AppUser user, ISession session, int id)
        {
            var tasks = RenewSessionListIds(user.Id, session, id)
                .Select(async m => await _movieService.GetReducedMovieAsync(m))
                .ToList();
            return (await Task.WhenAll(tasks)).ToList();
        }

        public async Task<List<MovieViewModel>> ShowMoviesListAsync(AppUser user, ISession session)
        {
            var list = session.Get<List<int>>(GetSessionListName(user.Id)) ?? new List<int>();
            var tasks = list.Select(async m => await _movieService.GetReducedMovieAsync(m))
                .ToList();
            return (await Task.WhenAll(tasks)).ToList();
        }

        private static List<int> RenewSessionListIds(string userId, ISession session, int id)
        {
            var list = session.Get<List<int>>(GetSessionListName(userId)) ?? new List<int>();

            //remove element if exists
            if (list.Contains(id)) list.Remove(id);

            //add to start
            list.Insert(0, id);

            //delete overflowed value
            if (list.Count > ValueToStore)
                list.RemoveAt(list.Count - 1);
            session.Set(GetSessionListName(userId), list);

            return list;
        }

        private static string GetSessionListName(string userId)
        {
            return $"SessionList_{userId}";
        }
    }
}