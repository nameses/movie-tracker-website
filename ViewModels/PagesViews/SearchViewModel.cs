namespace movie_tracker_website.ViewModels.PagesViews
{
    public class SearchViewModel
    {
        public AppUserViewModel? CurrentUser { get; set; }
        public List<MovieViewModel> Movies { get; set; }
        public string? Query { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string PageName { get; set; }
    }
}