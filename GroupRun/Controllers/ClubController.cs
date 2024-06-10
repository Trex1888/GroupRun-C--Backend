using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IWebHostEnvironment _environment;

        public ClubController(IClubRepository clubRepository, IWebHostEnvironment environment)
        {
            _clubRepository = clubRepository;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Club> clubs = await _clubRepository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubView)
        {
            if (clubView.Image == null)
            {
                ModelState.AddModelError("Image", "Image is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(clubView);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(clubView.Image!.FileName);

            string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                await clubView.Image.CopyToAsync(stream);
            }

            var club = new Club
            {
                Title = clubView.Title,
                Description = clubView.Description,
                Image = "/Images/" + newFileName,
                ClubCategory = clubView.ClubCategory,
                Address = new Address
                {
                    Street = clubView.Address.Street,
                    City = clubView.Address.City,
                    State = clubView.Address.State,
                }
            };

            _clubRepository.Add(club);
            _clubRepository.Save();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null) return View("Error");
            var clubVm = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory
            };
            return View(clubVm);
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ModelState.AddModelError("", "Failed to edit club");
        //        return View("Edit", clubVM);
        //    }

        //    var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);

        //    if (userClub == null)
        //    {
        //        return View("Error");
        //    }

        //    var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

        //    var club = new Club
        //    {
        //        Id = id,
        //        Title = clubVM.Title,
        //        Description = clubVM.Description,
        //        Image = photoResult.Url.ToString(),
        //        AddressId = clubVM.AddressId,
        //        Address = clubVM.Address,
        //    };

        //    _clubRepository.Update(club);
        //    return RedirectToAction("Index");
        //}
    }
}


//private readonly IPhotoService _photoService;

//public IActionResult Index()
//{
//    var clubs = _context.Clubs.ToList();
//    return View(clubs);
//}

//public async Task<IActionResult> Index()
//{
//    IEnumerable<Club> clubs = await _clubRepository.GetAll();
//    return View(clubs);
//}

//public IActionResult Detail(int id)
//{
//    Club club = _context.Clubs.FirstOrDefault(c => c.Id == id);
//    return View(club);
//}

//public async Task<IActionResult> Detail(int id)
//{
//    Club club = await _clubRepository.GetByIdAsync(id);
//    return View(club);
//}

//[HttpPost]
//public async Task<IActionResult> Create(CreateClubViewModel clubVM)
//{
//    if (ModelState.IsValid)
//    {
//        var result = await _photoService.AddPhotoAsync(clubVM.Image);

//        var club = new Club
//        {
//            Title = clubVM.Title,
//            Description = clubVM.Description,
//            Image = result.Url.ToString(),
//            ClubCategory = clubVM.ClubCategory,
//            Address = new Address
//            {
//                Street = clubVM.Address.Street,
//                City = clubVM.Address.City,
//                State = clubVM.Address.State,
//            }
//        };
//        _clubRepository.Add(club);
//        return RedirectToAction("Index");
//    }
//    else
//    {
//        ModelState.AddModelError("", "Photo upload failed");
//    }

//    return View(clubVM);
//}

