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
using static movie_tracker_website.Controllers.MoviePageController;

namespace movie_tracker_website.Services
{
    public class MoviePageService : IMoviePageService
    {
        private readonly IConfiguration _config;
        private readonly IMoviesList _moviesList;
        private readonly AuthDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMovieService _movieService;
        private readonly IMovieSessionListService _movieSessionListService;
        private readonly ITagService _tagService;

        public MoviePageService(IConfiguration config,
            IMoviesList moviesList,
            AuthDBContext context,
            UserManager<AppUser> userManager,
            IMovieService movieService,
            IMovieSessionListService movieSessionListService,
            ITagService tagService)
        {
            _context = context;
            _userManager = userManager;
            _movieService = movieService;
            _movieSessionListService = movieSessionListService;
            _tagService = tagService;
            _config = config;
            _moviesList = moviesList;
        }

        public async Task<MoviePageViewModel> GetRandomPageAsync(AppUser user, ISession session)
        {
            var movie = await this.GetRandomMovieAsync();
            if (movie == null) return null;

            Models.Movie movieFromDB = user.RelatedMovies.FirstOrDefault(m => m.ApiId == movie.Id);
            if (movieFromDB != null)
            {
                movie.IfWatched = movieFromDB.IfWatched;
                movie.IfFavourite = movieFromDB.IfFavourite;
                movie.IfToWatch = movieFromDB.IfToWatch;
            }
            //find similar movies to current movie
            var similarMovies = this.GetSimilarMovies(movie.Id);
            //process list of recently viewed movies in session
            var viewedMovies = await _movieSessionListService.ProcessMoviesListAsync(user, session, movie.Id);

            return new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies,
                ViewedMovies = viewedMovies
            };
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
            List<MovieViewModel>? viewedMovies = await _movieSessionListService.ProcessMoviesListAsync(user, session, id);

            //view models prepearing
            return new MoviePageViewModel()
            {
                CurrentUser = AppUserViewModel.ConvertToViewModel(user),
                Movie = movie,
                SimilarMovies = similarMovies,
                ViewedMovies = viewedMovies
            };
        }

        private List<MovieViewModel> GetSimilarMovies(int id)
        {
            using (var client = new TMDbClient(_config["APIKeys:TMDBAPI"]))
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

        private async Task<MovieViewModel> GetRandomMovieAsync()
        {
            using var client = new TMDbClient(_config["APIKeys:TMDBAPI"]);
            while (true)
            {
                var id = _moviesList.GetRandomMovieID();
                Movie movie = await client.GetMovieAsync(movieId: id, language: "en",
                    includeImageLanguage: null, MovieMethods.Videos | MovieMethods.Images);
                if (movie.Images.Backdrops.Count > 5 &&
                        movie.Videos.Results.Where(vid => vid.Type.Equals("Trailer")).Any())
                {
                    return await _movieService.GetMovieAsync(id);
                }
            }
        }

        //post methods
        public async Task<bool> ChangeMovieWatchedStatus(AppUser user, int ApiId, double? Rating = null)
        {
            Models.Movie? movieFromDB = user.RelatedMovies?.FirstOrDefault(m => m.ApiId == ApiId);
            if (movieFromDB != null)
            {
                movieFromDB.IfWatched = !movieFromDB.IfWatched;
                //set new time if movie is corrected to watched
                if (movieFromDB.IfWatched)
                {
                    user.UserStatistic.WatchedAmount++;
                    movieFromDB.TimeWatched = DateTime.Now;
                    //if watched - get out from to watch
                    movieFromDB.IfToWatch = false;
                }
            }
            //add new movie entry
            else
            {
                user.UserStatistic.WatchedAmount++;

                user.RelatedMovies.Add(new Models.Movie()
                {
                    ApiId = ApiId,
                    IfWatched = true,
                    TimeWatched = DateTime.Now,
                });
                //add tags for user
                _tagService.AddTagsForUser(user, ApiId);
            }
            var res = await _userManager.UpdateAsync(user);

            return res.Succeeded;
        }

        public async Task<bool> ChangeMovieFavouriteStatus(AppUser user, int ApiId, double? Rating = null)
        {
            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == ApiId);
            if (movieFromDB != null)
            {
                movieFromDB.IfFavourite = !movieFromDB.IfFavourite;
                if (movieFromDB.IfFavourite)
                {
                    user.UserStatistic.FavouriteAmount++;
                    //if film is favourite - then it watched
                    movieFromDB.IfWatched = true;
                }
            }
            //add new movie entry then
            else
            {
                user.UserStatistic.FavouriteAmount++;
                user.RelatedMovies.Add(new Models.Movie()
                {
                    ApiId = ApiId,
                    IfFavourite = true,
                    //if film is favourite - then it watched
                    IfWatched = true,
                    TimeWatched = DateTime.Now,
                });
            }

            var res = await _userManager.UpdateAsync(user);

            return res.Succeeded;
        }

        public async Task<bool> ChangeMovieToWatchStatus(AppUser user, int ApiId, double? Rating = null)
        {
            Models.Movie movieFromDB = user.RelatedMovies.Find(m => m.ApiId == ApiId);
            if (movieFromDB != null)
            {
                movieFromDB.IfToWatch = !movieFromDB.IfToWatch;
                if (movieFromDB.IfToWatch)
                {
                    user.UserStatistic.ToWatchAmount++;
                }
            }
            //add new movie entry then
            else
            {
                user.UserStatistic.ToWatchAmount++;
                user.RelatedMovies.Add(new Models.Movie()
                {
                    ApiId = ApiId,
                    IfToWatch = true,
                    TimeWatched = DateTime.Now,
                });
            }

            var res = await _userManager.UpdateAsync(user);

            return res.Succeeded;
        }
    }
}