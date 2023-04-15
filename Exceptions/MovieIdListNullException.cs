namespace movie_tracker_website.Exceptions
{
    public class MovieIdListNullException : Exception
    {
        public MovieIdListNullException() : base("The movie ID list cannot be null.")
        {
        }

        public MovieIdListNullException(string message) : base(message)
        {
        }

        public MovieIdListNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}