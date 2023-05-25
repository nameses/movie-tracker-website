namespace movie_tracker_website.ViewModels.PagesViews
{
    public class SearchViewModel
    {
        public AppUserViewModel? CurrentUser { get; set; }
        public List<MovieViewModel> Movies { get; set; }
        public string? Query { get; set; }
    }
}