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

        //text info

        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string OriginalLanguage { get; set; }
        public string? Tagline { get; set; }
        public string? Overview { get; set; }

        //values

        public string? Rating { get; set; }
        public string ReleaseYear { get; set; }
        public int? Duration { get; set; }

        //imgs&videos

        public string PosterPath { get; set; }
        public string? MainBackdropPath { get; set; }
        public List<string>? BackdropsPath { get; set; }
        public string? Trailer { get; set; }

        //credits

        public List<string>? Actors { get; set; }
        public string? Director { get; set; }

        //bools

        public Boolean IfWatched { get; set; }
        public Boolean IfFavourite { get; set; }
        public Boolean IfToWatch { get; set; }

        public static MovieViewModel convertToReducedMovieViewModel(dynamic inputMovie)
        {
            if (inputMovie is Movie || inputMovie is SearchMovie)
            {
                return new MovieViewModel
                {
                    Id = inputMovie.Id,
                    Title = inputMovie.Title,
                    ReleaseYear = inputMovie.ReleaseDate?.Year.ToString(),
                    PosterPath = inputMovie.PosterPath
                };
            }
            else throw new ArgumentException("Input must be of type Movie or SearchMovie");
        }

        public static MovieViewModel convertToSearchMovieViewModel(Movie inputMovie)
        {
            var director = inputMovie.Credits.Crew.FirstOrDefault(m => m.Job == "Director");
            return new MovieViewModel
            {
                Id = inputMovie.Id,
                Title = inputMovie.Title,
                OriginalTitle = inputMovie.OriginalTitle,
                OriginalLanguage = inputMovie.OriginalLanguage,
                ReleaseYear = inputMovie.ReleaseDate?.Year.ToString(),
                PosterPath = inputMovie.PosterPath,
                Director = director?.Name
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
                            .Where(data => data.AspectRatio > 1.7)
                            .Select(data => data.FilePath)
                            .Take(6).ToList(),
                Actors = inputMovie.Credits.Cast
                            .Select(cast => cast.Name)
                            .Take(10).ToList(),
            };
        }
    }
}