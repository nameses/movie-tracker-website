using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IProfileService
    {
        public ProfileViewModel GetProfileViewModel(AppUser user);
    }
}