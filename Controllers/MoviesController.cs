using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}