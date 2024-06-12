using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUserRepository userRepository, IHttpContextAccessor contextAccessor, IDashboardRepository dashboardRepository, UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _httpContextAccessor = contextAccessor;
            _dashboardRepository = dashboardRepository;
            _userManager = userManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();
            foreach (var user in users)
            {
                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    Pace = user.Pace,
                    Mileage = user.Mileage,
                    UserName = user.UserName,
                    ProfileImageUrl = user.ProfileImageUrl
                };
                result.Add(userViewModel);
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                Pace = user.Pace,
                Mileage = user.Mileage,
                UserName = user.UserName,
            };

            return View(userDetailViewModel);
        }

        [HttpGet]

        public async Task<IActionResult> EditProfile()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _dashboardRepository.GetUserById(curUserId);

            if (user == null)
            {
                return View("EditProfile");
            }

            var editProfileViewModel = new EditProfileViewModel()
            {
                Id = curUserId,
                Pace = user.Pace,
                Mileage = user.Mileage,
                City = user.City,
                State = user.State,
                ProfileImageUrl = user.ProfileImageUrl,
            };

            return View(editProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel editProfileViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit profile");
                return View("EditProfile", editProfileViewModel);
            }

            var user = await _dashboardRepository.GetByIdNoTracking(editProfileViewModel.Id);

            if (user == null)
            {
                return View(editProfileViewModel);
            }

            if (editProfileViewModel.Image != null) // only update profile image
            {
                var imageBytes = await ConvertImageToByteArrayAsync(editProfileViewModel.Image);
                var base64String = Convert.ToBase64String(imageBytes);

                user.ProfileImageUrl = base64String;

                await _userManager.UpdateAsync(user);

                return View(editProfileViewModel);
            }

            return RedirectToAction("Detail", "User", new { user.Id });
        }

        private static async Task<byte[]> ConvertImageToByteArrayAsync(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
