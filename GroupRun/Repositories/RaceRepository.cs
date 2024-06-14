using GroupRun.Data;
using GroupRun.Interfaces;
using GroupRun.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupRun.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly ApplicationDbContext _context;

        public RaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Race race)
        {
            await _context.AddAsync(race);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(Race race)
        {
            _context.Remove(race);
            return await SaveAsync();
        }

        public async Task<IEnumerable<Race>> GetAllAsync()
        {
            return await _context.Races.ToListAsync();
        }

        public async Task<Race> GetByIdAsync(int id)
        {
            return await _context.Races
                    .Include(i => i.Address)
                    .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Race> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Races
               .Include(i => i.Address)
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Race>> GetClubByCityAsync(string city)
        {
            return await _context.Races
                 .Where(c => c.Address.City.Contains(city))
                 .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Race race)
        {
            _context.Update(race);
            return await SaveAsync();
        }
    }
}
