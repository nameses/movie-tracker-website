using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IProfileService
    {
        public ProfileViewModel GetProfileViewModel(AppUser user);

        public ProfileViewModel GetProfileById(AppUser currentUser, string username);

        public void Follow(AppUser currentUser, string usernameToFollow);

        public void Unfollow(AppUser currentUser, string usernameToUnFollow);
    }
}