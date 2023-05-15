using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Utilities
{
    public interface IStatisticService
    {
        public StatisticViewModel GetUserStatistic(AppUser user);
    }
}