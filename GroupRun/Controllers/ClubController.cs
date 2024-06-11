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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);

            if (club == null)
            {
                return RedirectToAction("Index", "Club");
            }

            var clubView = new EditClubViewModel()
            {
                Id = club.Id,
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                ClubCategory = club.ClubCategory,
                Address = new Address
                {
                    Street = club.Address.Street,
                    City = club.Address.City,
                    State = club.Address.State
                },
                CurrentImage = club.Image
            };

            ViewData["ClubId"] = clubView.Id;
            ViewData["Image"] = club.Image;

            return View(clubView);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubView)
        {
            var club = await _clubRepository.GetByIdAsyncNoTracking(id);

            if (club == null)
            {
                return RedirectToAction("Index", "Club");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ClubId"] = club.Id;
                ViewData["Image"] = club.Image;

                return View(clubView);
            }

            string newFileName = club.Image;
            if (clubView.Image != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(clubView.Image.FileName);
                string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await clubView.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(club.Image))
                {
                    string oldImageFullPath = Path.Combine(_environment.WebRootPath, "Images", club.Image);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }
            }

            club.Title = clubView.Title;
            club.Description = clubView.Description;
            club.Address.Street = clubView.Address.Street;
            club.Address.City = clubView.Address.City;
            club.Address.State = clubView.Address.State;
            club.ClubCategory = clubView.ClubCategory;
            club.Image = "/Images/" + newFileName;

            _clubRepository.Update(club);
            _clubRepository.Save();

            return RedirectToAction("Index", "Club");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return RedirectToAction("Index", "Club");
            }

            return View(club);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return RedirectToAction("Index", "Club");
            }

            string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", club.Image);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            _clubRepository.Delete(club);
            _clubRepository.Save();

            return RedirectToAction("Index", "Club");
        }

    }
}

//public async Task<IActionResult> Delete(int id)
//{
//    var clubDetails = await _clubRepository.GetByIdAsync(id);
//    if (clubDetails == null) return View("Index");
//    return View(clubDetails);
//}

//[HttpPost, ActionName("Delete")]
//public async Task<IActionResult> DeleteClub(int id)
//{
//    var clubDetails = await _clubRepository.GetByIdAsync(id);
//    if (clubDetails == null) return View("Index");

//    _clubRepository.Delete(clubDetails);
//    return RedirectToAction("Index");
//}
//[HttpPost]
//public async Task<IActionResult> Delete(int id)
//{
//    var club = await _clubRepository.GetByIdAsync(id);
//    if (club == null)
//    {
//        return RedirectToAction("Index", "Club");
//    }

//    string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", club.Image);
//    if (System.IO.File.Exists(imageFullPath))
//    {
//        System.IO.File.Delete(imageFullPath);
//    }

//    _clubRepository.Delete(club);
//    _clubRepository.Save();

//    return RedirectToAction("Index", "Club");
//}

//public async Task<IActionResult> Edit(int id)
//{
//    var club = await _clubRepository.GetByIdAsync(id);
//    if (club == null) return View("Error");
//    var clubVm = new EditClubViewModel
//    {
//        Title = club.Title,
//        Description = club.Description,
//        AddressId = club.AddressId,
//        Address = club.Address,
//        URL = club.Image,
//        ClubCategory = club.ClubCategory
//    };
//    return View(clubVm);
//}

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

