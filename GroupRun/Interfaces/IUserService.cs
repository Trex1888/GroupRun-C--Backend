using GroupRun.ViewModels;

namespace GroupRun.Interfaces
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetAllUsersViewModels();
        Task<UserDetailViewModel> GetUserDetailViewModel(string id);
        Task<EditProfileViewModel> GetEditProfileViewModel(string userId);
        Task<bool> UpdateUserProfile(EditProfileViewModel editViewModel, IFormFile image);
    }
}
