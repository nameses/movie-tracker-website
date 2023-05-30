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
        private readonly IMovieSessionListService _movieSessionListService;

        public MoviePageService(IConfiguration config,
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

        public async Task<MoviePageViewModel> GetMoviePageAsync(int id, ISession session, AppUser user)
        {
            var movie = await _movieService.GetMovieAsync(id);
            if (movie == null) return null;

            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == movie.Id);
            if (movieFromDB != null)
            {
                movie.IfWatched = movieFromDB.IfWatched;
                movie.IfFavourite = movieFromDB.IfFavourite;
                movie.IfToWatch = movieFromDB.IfToWatch;
            }

            //find similar movies to current movie
            List<MovieViewModel> similarMovies = GetSimilarMovies(id);
            //proccess list of recently viewed movies in session
            List<MovieViewModel>? viewedMovies = await _movieSessionListService.ProcessMoviesListAsync(session, id);

            //view models prepearing
            return new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.convertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies,
                ViewedMovies = viewedMovies
            };
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

        public async Task<MovieViewModel> GetRandomMovieAsync()
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

            return await _movieService.GetMovieAsync(id);
        }
    }
}