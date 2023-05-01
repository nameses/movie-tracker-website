using Microsoft.IdentityModel.Tokens;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Services.common
{
    public class MovieSessionListService : IMovieSessionListService
    {
        private const string SessionViewedMoviesName = "viewedMovies";
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

        public List<MovieViewModel> ProcessSessionViewedMovies(ISession session, int id)
        {
            List<int> viewedMovies = RenewSessionListIds(session, id);

            //convert list of ids to list of models
            List<MovieViewModel> viewedMovieModels = new List<MovieViewModel>();
            foreach (var movieId in viewedMovies)
            {
                if (movieId == -1)
                    viewedMovieModels.Add(new MovieViewModel() { Id = -1 });
                else viewedMovieModels.Add(_movieService.GetReducedMovieById(movieId));
            }

            return viewedMovieModels;
        }

        public List<MovieViewModel>? ShowSessionViewedMovies(ISession session)
        {
            if (!session.Get<List<int>>(SessionViewedMoviesName).IsNullOrEmpty())
            {
                List<int> viewedMovies = session.Get<List<int>?>(SessionViewedMoviesName);
                //convert list of ids to list of models
                List<MovieViewModel> viewedMovieModels = new List<MovieViewModel>();
                foreach (var movieId in viewedMovies)
                {
                    if (movieId == -1)
                        viewedMovieModels.Add(new MovieViewModel() { Id = -1 });
                    else viewedMovieModels.Add(_movieService.GetReducedMovieById(movieId));
                }

                return viewedMovieModels;
            }
            return null;
        }

        private List<int> RenewSessionListIds(ISession session, int id)
        {
            List<int>? viewedMovies;

            if (session.Get<List<int>?>(SessionViewedMoviesName).IsNullOrEmpty())
                viewedMovies = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1 };
            else
                viewedMovies = session.Get<List<int>?>(SessionViewedMoviesName);

            //insert id to start of list and delete last element
            InsertNewId(viewedMovies, id);

            session.Set(SessionViewedMoviesName, viewedMovies);

            if (viewedMovies == null) throw new NullReferenceException();
            return viewedMovies;
        }

        private static void InsertNewId(List<int>? list, int id)
        {
            if (list == null) throw new NullReferenceException();

            if (list.Contains(id))
            {
                list.Remove(id);
                list.Insert(0, id);
            }
            else
            {
                list.Insert(0, id);
                list.RemoveAt(list.Count - 1);
            }

            if (list.Count > 8) throw new ArgumentException("List has exceeded its maximum size");
        }
    }
}