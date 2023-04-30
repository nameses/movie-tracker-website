using System.Text.Json;

namespace movie_tracker_website.Services.common
{
    public static class CookieExtensions
    {
        public static void Set<T>(this HttpResponse response, string key, T value)
        {
            var options = new CookieOptions
            {
                //HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            response.Cookies.Append(key, JsonSerializer.Serialize<T>(value), options);
        }

        public static T? Get<T>(this HttpRequest request, string key)
        {
            if (!request.Cookies.TryGetValue(key, out string value))
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}