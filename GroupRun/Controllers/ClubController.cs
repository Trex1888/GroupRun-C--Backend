using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroupRun.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository clubRepository, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _clubRepository = clubRepository;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
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

        [Authorize(Roles = "admin")] // Restrict access to admin users
        public IActionResult Create()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            var createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
            return View(createClubViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")] // Restrict access to admin users
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

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(clubView.Image.FileName);

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
                AppUserId = clubView.AppUserId,
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
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
        //    var createClubViewModel = new CreateClubViewModel { AppUserId = curUserId };
        //    return View(createClubViewModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(CreateClubViewModel clubView)
        //{
        //    if (clubView.Image == null)
        //    {
        //        ModelState.AddModelError("Image", "Image is required.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return View(clubView);
        //    }

        //    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(clubView.Image!.FileName);

        //    string imageFullPath = Path.Combine(_environment.WebRootPath, "Images", newFileName);
        //    using (var stream = System.IO.File.Create(imageFullPath))
        //    {
        //        await clubView.Image.CopyToAsync(stream);
        //    }

        //    var club = new Club
        //    {
        //        Title = clubView.Title,
        //        Description = clubView.Description,
        //        Image = "/Images/" + newFileName,
        //        ClubCategory = clubView.ClubCategory,
        //        AppUserId = clubView.AppUserId,
        //        Address = new Address
        //        {
        //            Street = clubView.Address.Street,
        //            City = clubView.Address.City,
        //            State = clubView.Address.State,
        //        }
        //    };

        //    _clubRepository.Add(club);
        //    _clubRepository.Save();

        //    return RedirectToAction("Index");
        //}

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

