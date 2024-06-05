using GroupRun.Data;
using GroupRun.Models;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class RaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RaceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //var races = _context.Races.ToList();
            List<Race> races = _context.Races.ToList();
            return View(races);
        }
    }
}
