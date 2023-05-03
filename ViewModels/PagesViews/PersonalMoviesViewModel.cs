namespace movie_tracker_website.ViewModels.PagesViews
{
    public class PersonalMoviesViewModel
    {
        public AppUserViewModel CurrentUser { get; set; }
        public List<MovieViewModel>? Movies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string PageName { get; set; }


    }
}