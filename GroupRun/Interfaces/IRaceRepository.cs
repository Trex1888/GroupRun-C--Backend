using GroupRun.Models;

namespace GroupRun.Interfaces
{
    public interface IRaceRepository
    {
        Task<IEnumerable<Race>> GetAllAsync();
        Task<Race> GetByIdAsync(int id);
        Task<Race> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Race>> GetClubByCityAsync(string city);
        Task<bool> AddAsync(Race race);
        Task<bool> UpdateAsync(Race race);
        Task<bool> DeleteAsync(Race race);
        Task<bool> SaveAsync();
    }
}
