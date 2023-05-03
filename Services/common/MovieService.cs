using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Services.common
{
    public class MovieService : IMovieService
    {
        private readonly IConfiguration _config;

        public MovieService(IConfiguration config)
        {
            _config = config;
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