using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface ICommunityService
    {
        public Task<CommunityViewModel> GetCommunityMembers(AppUser user, int page);
    }
}