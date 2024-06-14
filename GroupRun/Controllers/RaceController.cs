using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepo, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
        {
            _raceRepository = raceRepo;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Race> races = await _raceRepository.GetAllAsync();
            return View(races);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return NotFound();
            }
            return View(race);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var curUserID = _httpContextAccessor.HttpContext?.User.GetUserId();
            var createRaceViewModel = new CreateRaceViewModel { AppUserId = curUserID };
            return View(createRaceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceViewModel)
        {
            if (ModelState.IsValid)
            {
                if (raceViewModel.Image != null)
                {
                    var result = await _photoService.AddPhotoAsync(raceViewModel.Image);
                    if (result.Error != null)
                    {
                        ModelState.AddModelError("", "Photo upload failed");
                        return View(raceViewModel);
                    }

                    var race = new Race
                    {
                        Title = raceViewModel.Title,
                        Description = raceViewModel.Description,
                        Image = result.Url.ToString(),
                        RaceCategory = raceViewModel.RaceCategory,
                        AppUserId = raceViewModel.AppUserId,
                        Address = new Address
                        {
                            Street = raceViewModel.Address.Street,
                            City = raceViewModel.Address.City,
                            State = raceViewModel.Address.State,
                        }
                    };

                    await _raceRepository.AddAsync(race);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Please upload an image");
                }
            }

            return View(raceViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return NotFound();
            }

            var raceViewModel = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                AddressId = race.AddressId,
                Address = race.Address,
                URL = race.Image,
                RaceCategory = race.RaceCategory
            };

            return View(raceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel clubRaceModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club");
                return View("Edit", clubRaceModel);
            }

            var userClub = await _raceRepository.GetByIdAsyncNoTracking(id);
            if (userClub == null)
            {
                return NotFound();
            }

            if (clubRaceModel.Image != null)
            {
                var photoResult = await _photoService.AddPhotoAsync(clubRaceModel.Image);
                if (photoResult.Error != null)
                {
                    ModelState.AddModelError("Image", "Photo upload failed");
                    return View(clubRaceModel);
                }

                if (!string.IsNullOrEmpty(userClub.Image))
                {
                    _ = await _photoService.DeletePhotoAsync(userClub.Image);
                }

                var race = new Race
                {
                    Id = id,
                    Title = clubRaceModel.Title,
                    Description = clubRaceModel.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = clubRaceModel.AddressId,
                    Address = clubRaceModel.Address,
                    RaceCategory = clubRaceModel.RaceCategory,
                };

                await _raceRepository.UpdateAsync(race);
            }
            else
            {
                var race = new Race
                {
                    Id = id,
                    Title = clubRaceModel.Title,
                    Description = clubRaceModel.Description,
                    AddressId = clubRaceModel.AddressId,
                    Address = clubRaceModel.Address,
                };

                await _raceRepository.UpdateAsync(race);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails == null)
            {
                return NotFound();
            }

            return View(raceDetails);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null)
            {
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(race.Image))
            {
                var deletionResult = await _photoService.DeletePhotoAsync(race.Image);
                if (deletionResult.Result != "ok")
                {
                    ModelState.AddModelError("", "Failed to delete photo");
                    // Handle deletion failure (log, show error message, etc.)
                }
            }

            await _raceRepository.DeleteAsync(race);

            return RedirectToAction("Index");
        }
    }
}
