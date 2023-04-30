using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using movie_tracker_website.Data;
using movie_tracker_website.Exceptions;
using System.ComponentModel;
using System.Xml.Linq;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace movie_tracker_website.Utilities
{
    public class MoviesList : IMoviesList
    {
        private readonly ILogger<MoviesList> _logger;
        private readonly IConfiguration _config;

        private static List<int> MoviesIdsList { get; set; }
        private const int PageNumber = 15;

        public MoviesList(ILogger<MoviesList> logger,
            IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Gets list of movie ids of size PageNumber from TMDBAPI.
        /// </summary>
        /// <returns>List of movie ids</returns>
        /// <exception cref="MovieIdListNullException">Thrown if list with IDs is not found.</exception>
        private List<int> GetMovieList()
        {
            var moviesIdList = new List<int>();
            using (TMDbClient client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
            {
                for (int page = 1; page <= PageNumber; page++)
                {
                    var topRatedList = client.GetMovieTopRatedListAsync(page: page).Result;

                    foreach (var result in topRatedList.Results)
                    {
                        if (result.OriginalLanguage != "ja" &&
                            result.OriginalLanguage != "hi" &&
                            result.OriginalLanguage != "ko")
                            moviesIdList.Add(result.Id);
                    }
                }
            }
            if (moviesIdList.Count == 0)
                throw new MovieIdListNullException();
            return moviesIdList;
        }

        private void DownloadMovieList()
        {
            try
            {
                MoviesIdsList = GetMovieList();
            }
            catch (MovieIdListNullException e)
            {
                _logger.LogError(e, "Movie IDs list cannot be null");

                throw new MovieIdListNullException();
            }
        }

        /// <summary>
        /// Gets random movie id from top movies of TMDB
        /// </summary>
        /// <returns>Returns random movie ID form top (20*PageNumber) movies in TMDB</returns>
        /// <exception cref="MovieNotFound"></exception>
        public int GetRandomMovieID()
        {
            try
            {
                if (MoviesIdsList == null || MoviesIdsList.Count == 0)
                    DownloadMovieList();
                return MoviesIdsList[new Random().Next(0, MoviesIdsList.Count - 1)];
            }
            catch (MovieIdListNullException e)
            {
                _logger.LogError(e, "Movie IDs list cannot be null");

                throw new MovieNotFound();
            }
        }
    }
}