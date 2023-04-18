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
                //get all info about movie we need, without imgs
                Movie movie = client.GetMovieAsync(movieId: id,
                    language: "uk-UK", includeImageLanguage: null,
                    MovieMethods.Credits).Result;
                //get imgs of film without param "language"
                ImagesWithId movieImages = client.GetMovieImagesAsync(movieId: id, language: "null").Result;

                if (movie == null || movieImages == null) return null;

                movieView = MovieViewModel.convertToViewModel(movie, movieImages);
                movieView = CorrectNullValues(movieView);
            }
            return movieView;
        }

        public MovieViewModel GetRandomMovie()
        {
            MovieViewModel movieView;
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                int randomId = _moviesList.GetRandomMovieID();
                //get all info about movie we need without imgs
                Movie movie = client.GetMovieAsync(movieId: randomId,
                    language: "uk-UK", includeImageLanguage: null,
                    MovieMethods.Credits).Result;
                //get imgs of film without param "language"
                ImagesWithId movieImages = client.GetMovieImagesAsync(movieId: randomId, language: "null").Result;
                while (true)
                {
                    if (movieImages.Backdrops.Count < 3)
                        return GetRandomMovie();
                    else break;
                }
                movieView = MovieViewModel.convertToViewModel(movie, movieImages);

                movieView = CorrectNullValues(movieView);
                if (movieView == null) return GetRandomMovie();
            }
            return movieView;
        }

        private MovieViewModel CorrectNullValues(MovieViewModel movieViewModel)
        {
            if (movieViewModel.Title == "" || movieViewModel.Overview == "" || movieViewModel.Tagline == "")
                SetTextFields(movieViewModel);
            return movieViewModel;
        }

        private void SetTextFields(MovieViewModel movieViewModel)
        {
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                Movie movie = client.GetMovieAsync(movieId: movieViewModel.Id,
                    language: "en-US").Result;
                movieViewModel.Overview = movie.Overview;
                movieViewModel.Tagline = movie.Tagline;
                movieViewModel.Title = movie.Title;
            }
        }
    }
}