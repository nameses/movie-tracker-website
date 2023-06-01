using Microsoft.AspNetCore.Identity;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Controllers;
using movie_tracker_website.Data;
using movie_tracker_website.Utilities;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Services.common
{
    public class StatisticService : IStatisticService
    {
        private readonly AuthDBContext _context;
        private readonly ILogger<MoviePageController> _logger;

        public StatisticService(ILogger<MoviePageController> logger,
                AuthDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public StatisticViewModel GetUserStatistic(AppUser user)
        {
            return new StatisticViewModel()
            {
                StatisticDict = new Dictionary<string, int>()
                {
                    { "переглянуто", user.RelatedMovies.Where(m => m.IfWatched).Count() },
                    { "цього року", user.RelatedMovies
                        .Where(m => m.IfWatched)
                        .Where(m => m.TimeWatched.Year==DateTime.Now.Year)
                        .Count()
                    },
                    { "улюблених", user.RelatedMovies.Where(m => m.IfFavourite).Count()},
                    { "до перегляду", user.RelatedMovies.Where(m => m.IfToWatch).Count()}
                }
            };
        }
    }
}