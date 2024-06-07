using GroupRun.Data;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;
		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var clubs = _context.Clubs.ToList();
			return View(clubs);
		}
	}
}