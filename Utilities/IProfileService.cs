using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels.PagesViews;

namespace movie_tracker_website.Utilities
{
    public interface IProfileService
    {
        public Task<ProfileViewModel> GetProfileAsync(AppUser user);

        public Task<ProfileViewModel> GetProfileByUsernameAsync(AppUser currentUser, string username);

        public void Follow(AppUser currentUser, string usernameToFollow);

        public void Unfollow(AppUser currentUser, string usernameToUnFollow);
    }
}