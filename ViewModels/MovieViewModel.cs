using Microsoft.IdentityModel.Tokens;
using System.Linq;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace movie_tracker_website.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReleaseYear { get; set; }
        public int? Duration { get; set; }
        public string? Tagline { get; set; }
        public string? Overview { get; set; }
        public string? Rating { get; set; }
        public string PosterPath { get; set; }
        public string? MainBackdropPath { get; set; }
        public List<string>? BackdropsPath { get; set; }
        public string? Trailer { get; set; }
        public List<string>? Actors { get; set; }
        public Boolean IfWatched { get; set; } = false;
        public Boolean? IfFavourite { get; set; }
        public Boolean? IfToWatch { get; set; }

        public static MovieViewModel convertToSimilarMovieViewModel(SearchMovie inputMovie)
        {
            return new MovieViewModel
            {
                Id = inputMovie.Id,
                Title = inputMovie.Title,
                ReleaseYear = inputMovie.ReleaseDate?.Year.ToString(),
                PosterPath = inputMovie.PosterPath
            };
        }

        public static MovieViewModel convertToViewModel(Movie inputMovie, ImagesWithId images)
        {
            return new MovieViewModel
            {
                Id = inputMovie.Id,
                Title = inputMovie.Title,
                ReleaseYear = inputMovie.ReleaseDate.Value.Year.ToString(),
                Duration = inputMovie.Runtime.Value,
                Tagline = inputMovie.Tagline,
                Overview = inputMovie.Overview,
                Rating = inputMovie.VoteAverage.ToString().Replace(',', '.'),
                PosterPath = inputMovie.PosterPath,
                MainBackdropPath = inputMovie.BackdropPath,
                BackdropsPath = images.Backdrops
                            //.OrderBy(data => data.VoteAverage)
                            //.OrderBy(data => data.VoteCount)
                            .Where(data => data.AspectRatio > 1.7)
                            //.OrderBy(a => Guid.NewGuid())
                            .Select(data => data.FilePath)
                            .Take(6).ToList(),
                Actors = inputMovie.Credits.Cast
                            .Select(cast => cast.Name)
                            .Take(10).ToList(),
            };
        }
    }
}