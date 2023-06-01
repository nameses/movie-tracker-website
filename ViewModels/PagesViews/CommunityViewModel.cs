namespace movie_tracker_website.ViewModels.PagesViews
{
    public class CommunityViewModel
    {
        public AppUserViewModel? CurrentUser { get; set; }
        public List<AppUserViewModel>? Users { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}