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

        public MoviePageService(IConfiguration config,
            IMoviesList moviesList,
            AuthDBContext context)
        {
            _context = context;
            _config = config;
            _moviesList = moviesList;
        }

        public List<MovieViewModel> ProccessSessionViewedMovies(ISession session, int id)
        {
            List<int> viewedMovies = new List<int>();
            //if session is not null -> get it
            if (!session.Get<List<int>>(SessionViewedMoviesName).IsNullOrEmpty())
                viewedMovies = session.Get<List<int>>(SessionViewedMoviesName);
            //session processing
            viewedMovies.Add(id);
            session.Set(SessionViewedMoviesName, viewedMovies);
            //convert list of ids to list of models
            List<MovieViewModel> viewedMovieModels = new List<MovieViewModel>();
            foreach (var movieId in viewedMovies)
                viewedMovieModels.Add(GetReducedMovieById(movieId));

            return viewedMovieModels;
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

        public MovieViewModel? GetMovieById(int id)
        {
            MovieViewModel movieView;
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                //get all info about movie we need without imgs
                Movie movieUA = client.GetMovieAsync(movieId: id,
                    language: "uk-UK", includeImageLanguage: null,
                    MovieMethods.Credits).Result;

                Movie movieEN = client.GetMovieAsync(movieId: id,
                    language: "en", includeImageLanguage: null,
                    MovieMethods.Videos | MovieMethods.Images).Result;
                //get imgs of film without param "language"
                ImagesWithId movieImages = client.GetMovieImagesAsync(movieId: id,
                    language: "null", includeImageLanguage: null).Result;

                movieView = MovieViewModel.convertToViewModel(movieUA, movieImages);

                movieView = CorrectNullValues(movieView, movieEN);
            }
            return movieView;
        }

        public MovieViewModel? GetReducedMovieById(int id)
        {
            MovieViewModel? model = null;
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                var movie = client.GetMovieAsync(id, language: "uk-UK").Result;
                model = MovieViewModel.convertToReducedMovieViewModel(movie);

                //if movie title == null then find new movie title
                model.Title ??= client.GetMovieAsync(id, language: "en").Result.Title;
            }
            return model;
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

            return this.GetMovieById(id);
        }

        private MovieViewModel CorrectNullValues(MovieViewModel inputMovie, Movie movieEN)
        {
            var videos = movieEN.Videos.Results;
            string videoKey = null;
            if (videos.Count > 0)
            {
                videoKey = videos.Where(vid => vid.Type.Equals("Trailer"))
                    .Where(video => video.Site.Equals("YouTube"))
                    .Take(1).First().Key;
            }
            if (inputMovie.Title == "") inputMovie.Title = movieEN.Title;
            if (inputMovie.Overview == "") inputMovie.Overview = movieEN.Overview;
            if (inputMovie.Tagline == "") inputMovie.Tagline = movieEN.Tagline;
            if (inputMovie.Trailer == null && videoKey != null) inputMovie.Trailer = videoKey;
            if (inputMovie.PosterPath == "") inputMovie.PosterPath = movieEN.PosterPath;
            if (inputMovie.MainBackdropPath == "") inputMovie.MainBackdropPath = movieEN.BackdropPath;

            return inputMovie;
        }
    }
}