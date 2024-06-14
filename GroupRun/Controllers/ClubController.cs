using GroupRun.Interfaces;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubService _clubService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubService clubService, IHttpContextAccessor httpContextAccessor)
        {
            _clubService = clubService ?? throw new ArgumentNullException(nameof(clubService));
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clubs = await _clubService.GetAllClubsAsync();
            return View(clubs);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var club = await _clubService.GetClubByIdAsync(id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var curUserID = _httpContextAccessor.HttpContext?.User.GetUserId();
            var createRaceViewModel = new CreateClubViewModel { AppUserId = curUserID };
            return View(createRaceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubViewModel)
        {
            if (ModelState.IsValid)
            {
                var success = await _clubService.CreateClubAsync(clubViewModel);
                if (success)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Photo upload failed or missing image");
            }

            return View(clubViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var clubViewModel = await _clubService.GetClubForEditAsync(id);
            if (clubViewModel == null)
            {
                return NotFound();
            }

            return View(clubViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubViewModel);
            }

            var success = await _clubService.UpdateClubAsync(id, clubViewModel);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await _clubService.GetClubForDeleteAsync(id);
            if (clubDetails == null)
            {
                return NotFound();
            }

            return View(clubDetails);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _clubService.DeleteClubAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete club or photo");
            }

            return RedirectToAction("Index");
        }
    }
}