using movie_tracker_website.Areas.Identity.Data;

namespace movie_tracker_website.Utilities
{
    public interface ITagService
    {
        public void AddTagsForUser(AppUser user, int movieId);
        public List<string?> GetImportantTags(AppUser user, int amount);
    }
}