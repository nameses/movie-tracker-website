using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Services;
using movie_tracker_website.Services.common;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;
using System.Collections.Generic;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Languages;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Services
{
    public class MoviePageService : IMoviePageService
    {
        private const string SessionViewedMoviesName = "viewedMovies";
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly AuthDBContext _context;
        private readonly IMovieService _movieService;

        public MoviePageService(IConfiguration config,
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
                List<int> viewedMovies = session.Get<List<int>>(SessionViewedMoviesName);
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

        public List<MovieViewModel> GetSimilarMovies(int id)
        {
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                return client.GetMovieSimilarAsync(id, page: 0)
                    .Result
                    .Results
                    .Where(m => m.Title != null && m.Overview != null && m.PosterPath != null && m.BackdropPath != null)
                    .Take(8)
                    .Select(MovieViewModel.convertToReducedMovieViewModel)
                    .ToList();
            }
        }

        public MovieViewModel GetRandomMovie()
        {
            int id;
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                while (true)
                {
                    id = _moviesList.GetRandomMovieID();
                    Movie movie = client.GetMovieAsync(movieId: id,
                        language: "en", includeImageLanguage: null,
                        MovieMethods.Videos | MovieMethods.Images).Result;
                    if (movie.Images.Backdrops.Count > 5 &&
                        movie.Videos.Results.Where(vid => vid.Type.Equals("Trailer"))
                        .Count() > 0) break;
                }
            }

            return _movieService.GetMovieById(id);
        }

        private List<int> RenewSessionListIds(ISession session, int id)
        {
            List<int> viewedMovies;

            if (session.Get<List<int>>(SessionViewedMoviesName).IsNullOrEmpty())
                viewedMovies = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1 };
            else
                viewedMovies = session.Get<List<int>>(SessionViewedMoviesName);

            //insert id to start of list and delete last element
            InsertNewId(viewedMovies, id);

            session.Set(SessionViewedMoviesName, viewedMovies);
            return viewedMovies;
        }

        private void InsertNewId(List<int> list, int id)
        {
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