using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUserService userService, UserManager<AppUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var usersViewModel = await _userService.GetAllUsersViewModels();
            return View(usersViewModel);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var userDetailViewModel = await _userService.GetUserDetailViewModel(id);
            if (userDetailViewModel == null)
            {
                return RedirectToAction("Index", "Home"); // Or any other appropriate action
            }

            return View(userDetailViewModel);
        }

        [HttpGet("users/edit")]
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User); // Get the current user's ID

            var editProfileViewModel = await _userService.GetEditProfileViewModel(userId);
            if (editProfileViewModel == null)
            {
                return NotFound();
            }

            return View(editProfileViewModel);
        }

        [HttpPost("users/edit")]
        [ValidateAntiForgeryToken] // Always use AntiForgeryToken in POST requests for security
        public async Task<IActionResult> EditProfile(EditProfileViewModel editViewModel, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel); // Return the view with errors if model state is invalid
            }

            var success = await _userService.UpdateUserProfile(editViewModel, image);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to update profile"); // Add model-level error
                return View(editViewModel);
            }

            return RedirectToAction("Detail", "User", new { id = editViewModel.Id });
        }
    }
}
