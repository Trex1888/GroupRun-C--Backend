using GroupRun.Data;
using GroupRun.Interfaces;
using GroupRun.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupRun.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Club>> GetAllUserClubs()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (curUser == null)
            {
                return new List<Club>();
            }

            var userClubs = _context.Clubs.Where(r => r.AppUser.Id == curUser);

            return await userClubs.ToListAsync();
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (curUser == null)
            {
                return new List<Race>();
            }

            var userRaces = _context.Races.Where(r => r.AppUser.Id == curUser);

            return await userRaces.ToListAsync();
        }

        public async Task<AppUser?> GetUserById(string id)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            var user = await _context.Users.FindAsync(id);

            return user;
        }


        public async Task<AppUser?> GetByIdNoTracking(string id)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            var user = await _context.Users
                .Where(u => u.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> Update(AppUser user)
        {
            _context.Users.Update(user);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }
    }
}
