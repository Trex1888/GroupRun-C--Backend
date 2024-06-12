using GroupRun.Models;

namespace GroupRun.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<Race>> GetAllUserRaces();
        Task<List<Club>> GetAllUserClubs();
        Task<AppUser?> GetUserById(string id);
        Task<AppUser?> GetByIdNoTracking(string id);
        Task<bool> Update(AppUser user);
        Task<bool> Save();
    }
}
