﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_tracker_website.Areas.Identity.Data;
using movie_tracker_website.ViewModels;

namespace movie_tracker_website.Controllers
{
    [Authorize]
    public class PersonalMoviesController : Controller
    {
        private readonly ILogger<PersonalMoviesController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonalMoviesController(ILogger<PersonalMoviesController> logger,
                UserManager<AppUser> userManager,
                IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var appUser = new AppUserViewModel()
            {
                Username = user.UserName,
                Email = user.Email,
                ImagePath = user.ImagePath
            };
            return View(appUser);
        }
    }
}