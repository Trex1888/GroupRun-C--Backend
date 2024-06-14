using GroupRun.Models;
using GroupRun.ViewModels;

namespace GroupRun.Interfaces
{
    public interface IClubService
    {
        Task<IEnumerable<Club>> GetAllClubsAsync();
        Task<Club?> GetClubByIdAsync(int id);
        Task<bool> CreateClubAsync(CreateClubViewModel clubViewModel);
        Task<EditClubViewModel?> GetClubForEditAsync(int id);
        Task<bool> UpdateClubAsync(int id, EditClubViewModel clubViewModel);
        Task<Club?> GetClubForDeleteAsync(int id);
        Task<bool> DeleteClubAsync(int id);
    }
}
