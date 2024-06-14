using GroupRun.Models;

namespace GroupRun.Interfaces
{
    public interface IClubRepository
    {
        Task<IEnumerable<Club>> GetAllAsync();
        Task<Club> GetByIdAsync(int id);
        Task<Club> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Club>> GetClubByCityAsync(string city);
        Task<bool> AddAsync(Club club);
        Task<bool> UpdateAsync(Club club);
        Task<bool> DeleteAsync(Club club);
        Task<bool> SaveAsync();
    }
}

