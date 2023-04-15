namespace movie_tracker_website.Exceptions
{
    public class MovieNotFound : Exception
    {
        public MovieNotFound() : base("The movie ID list cannot be null.")
        {
        }

        public MovieNotFound(string message) : base(message)
        {
        }

        public MovieNotFound(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}