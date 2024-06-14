using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace GroupRun.Business
{
    public class UserHandler : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;
        private readonly UserManager<AppUser> _userManager;

        public UserHandler(IUserRepository userRepository, IPhotoService photoService, UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _photoService = photoService;
            _userManager = userManager;
        }

        public async Task<List<UserViewModel>> GetAllUsersViewModels()
        {
            var users = await _userRepository.GetAllUsers();
            return users.Select(user => MapToUserViewModel(user)).ToList();
        }

        public async Task<EditProfileViewModel> GetEditProfileViewModel(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null ? MapToEditProfileViewModel(user) : null;
        }

        public async Task<UserDetailViewModel> GetUserDetailViewModel(string id)
        {
            var user = await _userRepository.GetUserById(id);
            return user != null ? MapToUserDetailViewModel(user) : null;
        }

        public async Task<bool> UpdateUserProfile(EditProfileViewModel editViewModel, IFormFile image)
        {
            var user = await _userManager.FindByIdAsync(editViewModel.Id);
            if (user == null)
                return false;

            // Update user properties based on the ViewModel
            user.City = editViewModel.City;
            user.State = editViewModel.State;
            user.Pace = editViewModel.Pace;
            user.Mileage = editViewModel.Mileage;
            user.Description = editViewModel.Description;
            user.UserName = editViewModel.UserName;

            // Handle profile image upload if necessary
            if (image != null)
            {
                var photoResult = await _photoService.AddPhotoAsync(image);

                if (photoResult.Error != null)
                    return false;

                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);

                user.ProfileImageUrl = photoResult.Url.ToString();
            }

            // Update user in the database
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private UserViewModel MapToUserViewModel(AppUser user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Pace = user.Pace,
                City = user.City,
                State = user.State,
                Mileage = user.Mileage,
                UserName = user.UserName,
                Description = user.Description,
                ProfileImageUrl = user.ProfileImageUrl ?? "/Images/20240611013310576.jpg"
            };
        }

        private UserDetailViewModel MapToUserDetailViewModel(AppUser user)
        {
            return new UserDetailViewModel
            {
                Id = user.Id,
                Pace = user.Pace,
                City = user.City,
                State = user.State,
                Mileage = user.Mileage,
                UserName = user.UserName,
                Description = user.Description,
                ProfileImageUrl = user.ProfileImageUrl ?? "/Images/20240611013310576.jpg"
            };
        }

        private EditProfileViewModel MapToEditProfileViewModel(AppUser user)
        {
            return new EditProfileViewModel
            {
                Id = user.Id,
                Pace = user.Pace,
                City = user.City,
                State = user.State,
                Mileage = user.Mileage,
                Description = user.Description,
                ProfileImageUrl = user.ProfileImageUrl,
                UserName = user.UserName
            };
        }
    }
}
