using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceController(IRaceRepository raceRepo, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepo;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAll();
            return View(races);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Race race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserId };
            return View(createRaceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceView)
        {
            if (raceView.Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(raceView);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(raceView.Image!.FileName);

            string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                await raceView.Image.CopyToAsync(stream);
            }

            var race = new Race
            {
                Title = raceView.Title,
                Description = raceView.Description,
                Image = "/Images/" + newFileName,
                RaceCategory = raceView.RaceCategory,
                AppUserId = raceView.AppUserId,
                Address = new Address
                {
                    Street = raceView.Address.Street,
                    City = raceView.Address.City,
                    State = raceView.Address.State,
                }
            };

            _raceRepository.Add(race);
            _raceRepository.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);

            if (race == null)
            {
                return RedirectToAction("Index", "Race");
            }

            var raceView = new EditRaceViewModel()
            {
                Id = race.Id,
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                RaceCategory = race.RaceCategory,
                Address = new Address
                {
                    Street = race.Address.Street,
                    City = race.Address.City,
                    State = race.Address.State
                },
                CurrentImage = race.Image
            };

            ViewData["RaceId"] = raceView.Id;
            ViewData["Image"] = race.Image;

            return View(raceView);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceView)
        {
            var race = await _raceRepository.GetByIdAsyncNoTracking(id);

            if (race == null)
            {
                return RedirectToAction("Index", "Club");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClubId"] = race.Id;
                ViewData["Image"] = race.Image;

                return View(raceView);
            }

            string newFileName = race.Image;
            if (raceView.Image != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(raceView.Image.FileName);
                string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await raceView.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(race.Image))
                {
                    string oldImageFullPath = Path.Combine(_environment.WebRootPath, "Images", race.Image);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }
            }

            race.Title = raceView.Title;
            race.Description = raceView.Description;
            race.Address.Street = raceView.Address.Street;
            race.Address.City = raceView.Address.City;
            race.Address.State = raceView.Address.State;
            race.RaceCategory = raceView.RaceCategory;
            race.Image = "/Images/" + newFileName;

            _raceRepository.Update(race);
            _raceRepository.Save();

            return RedirectToAction("Index", "Race");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return RedirectToAction("Index", "Race");
            }

            return View(race);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return RedirectToAction("Index", "Race");
            }

            string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", race.Image);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            _raceRepository.Delete(race);
            _raceRepository.Save();

            return RedirectToAction("Index", "Race");
        }
    }
}
