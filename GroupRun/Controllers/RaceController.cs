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
        // private readonly IPhotoService _photoService;

        public RaceController(IRaceRepository raceRepo, IWebHostEnvironment environment)
        {
            _raceRepository = raceRepo;
            _environment = environment;
            //  _photoService = photoService;

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
            return View();
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

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _photoService.AddPhotoAsync(raceVM.Image);

        //        var race = new Race
        //        {
        //            Title = raceVM.Title,
        //            Description = raceVM.Description,
        //            Image = result.Url.ToString(),
        //            RaceCategory = raceVM.RaceCategory,
        //            Address = new Address
        //            {
        //                Street = raceVM.Address.Street,
        //                City = raceVM.Address.City,
        //                State = raceVM.Address.State,
        //            }
        //        };
        //        _raceRepository.Add(race);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Photo upload failed");
        //    }

        //    return View(raceVM);
        //}
    }
}
