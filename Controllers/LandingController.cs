using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.Data;

namespace movie_tracker_website.Controllers
{
    public class LandingController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthDBContext _context;
        private readonly SignInManager<AppUser> _signInManager;

        public LandingController(UserManager<AppUser> userManager,
            AuthDBContext context,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                if (user == null) return View();

                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View();
        }
    }
}