using GroupRun.Data;
using GroupRun.Interfaces;
using GroupRun.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupRun.Repositories
{
    public class ClubRepository : IClubRepository
    {
        private readonly ApplicationDbContext _context;

        public ClubRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddAsync(Club club)
        {
            await _context.AddAsync(club);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(Club club)
        {
            _context.Remove(club);
            return await SaveAsync();
        }

        public async Task<IEnumerable<Club>> GetAllAsync()
        {
            return await _context.Clubs.ToListAsync();
        }

        public async Task<Club> GetByIdAsync(int id)
        {
            return await _context.Clubs
                .Include(i => i.Address)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Club> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Clubs
                  .Include(i => i.Address)
                  .AsNoTracking()
                  .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Club>> GetClubByCityAsync(string city)
        {
            return await _context.Clubs
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

        public async Task<bool> UpdateAsync(Club club)
        {
            _context.Update(club);
            return await SaveAsync();
        }
    }
}




