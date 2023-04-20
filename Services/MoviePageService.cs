using Microsoft.AspNetCore.Http.HttpResults;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;
using movie_tracker_website.ViewModels.PagesViews;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Services
{
    public class MoviePageService : IMoviePageService
    {
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;

        public MoviePageService(IConfiguration config,
            IMoviesList moviesList)
        {
            _config = config;
            _moviesList = moviesList;
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
                    .Select(MovieViewModel.convertToSimilarMovieViewModel)
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
            if(videos.Count > 0)
            {
                videoKey = videos.Where(vid => vid.Type.Equals("Trailer"))
                    .Where(video => video.Site.Equals("YouTube"))
                    .Take(1).First().Key;
            }
            if (inputMovie.Title == "") inputMovie.Title = movieEN.Title;
            if (inputMovie.Overview == "") inputMovie.Overview = movieEN.Overview;
            if (inputMovie.Tagline == "") inputMovie.Tagline = movieEN.Tagline;
            if (inputMovie.Trailer == null && videoKey!=null) inputMovie.Trailer = videoKey;
            if (inputMovie.PosterPath == "") inputMovie.PosterPath = movieEN.PosterPath;
            if (inputMovie.MainBackdropPath == "") inputMovie.MainBackdropPath = movieEN.BackdropPath;

            return inputMovie;
        }
    }
}