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

        public async Task<MovieViewModel> GetMovieAsync(int id)
        {
            MovieViewModel movieView;
            using (var client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                //get all info about movie we need without imgs
                Movie movieUA = client.GetMovieAsync(movieId: id,
                    language: "uk-UK", includeImageLanguage: null,
                    MovieMethods.Credits).Result;

                Movie movieEN = client.GetMovieAsync(movieId: id,
                    language: "en", includeImageLanguage: null,
                    MovieMethods.Videos | MovieMethods.Images).Result;
                //get imgs of film without param "language"
                ImagesWithId movieImages = await client.GetMovieImagesAsync(movieId: id,
                    language: "null", includeImageLanguage: null);

                movieView = MovieViewModel.convertToViewModel(movieUA, movieImages);

                movieView = CorrectNullValues(movieView, movieEN);
            }
            return movieView;
        }

        public async Task<MovieViewModel> GetReducedMovieAsync(int id)
        {
            MovieViewModel model;
            using (TMDbClient client = new(_config["APIKeys:TMDBAPI"]))
            {
                var movie = await client.GetMovieAsync(id, language: "uk-UK");
                model = MovieViewModel.convertToReducedMovieViewModel(movie);

                //if movie title == null then find new movie title
                model.Title ??= (await client.GetMovieAsync(id, language: "en")).Title;
            }
            return model;
        }

        private static MovieViewModel CorrectNullValues(MovieViewModel inputMovie, Movie movieEN)
        {
            var videos = movieEN.Videos.Results;
            string videoKey;
            //get trailer from youtube
            if (videos.Count > 0)
            {
                videoKey = videos.Where(vid => vid.Type.Equals("Trailer"))
                    .Where(video => video.Site.Equals("YouTube"))
                    .First().Key;

                if (inputMovie.Trailer == null && videoKey != null) inputMovie.Trailer = videoKey;
            }
            if (inputMovie.Title == "") inputMovie.Title = movieEN.Title;
            if (inputMovie.Overview == "") inputMovie.Overview = movieEN.Overview;
            if (inputMovie.Tagline == "") inputMovie.Tagline = movieEN.Tagline;
            if (inputMovie.PosterPath == "") inputMovie.PosterPath = movieEN.PosterPath;
            if (inputMovie.MainBackdropPath == "") inputMovie.MainBackdropPath = movieEN.BackdropPath;

            return inputMovie;
        }
    }
}