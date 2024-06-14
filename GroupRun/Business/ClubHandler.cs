using GroupRun.Interfaces;
using GroupRun.Models;
using GroupRun.ViewModels;

namespace GroupRun.Business
{
    public class ClubHandler : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;

        public ClubHandler(IClubRepository clubRepository, IPhotoService photoService)
        {
            //_clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
            //_photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));
            _clubRepository = clubRepository;
            _photoService = photoService;
        }

        public async Task<bool> CreateClubAsync(CreateClubViewModel clubViewModel)
        {
            if (clubViewModel.Image != null)
            {
                var result = await _photoService.AddPhotoAsync(clubViewModel.Image);
                if (result.Error != null)
                {
                    return false;
                }

                var club = new Club
                {
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    Image = result.Url.ToString(),
                    ClubCategory = clubViewModel.ClubCategory,
                    AppUserId = clubViewModel.AppUserId,
                    Address = new Address
                    {
                        Street = clubViewModel.Address.Street,
                        City = clubViewModel.Address.City,
                        State = clubViewModel.Address.State,
                    }
                };

                return await _clubRepository.AddAsync(club);
            }

            return false;
        }

        public async Task<bool> DeleteClubAsync(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(club.Image))
            {
                var deletionResult = await _photoService.DeletePhotoAsync(club.Image);
                if (deletionResult.Result != "ok")
                {
                    return false;
                }
            }

            return await _clubRepository.DeleteAsync(club);
        }

        public async Task<IEnumerable<Club>> GetAllClubsAsync()
        {
            return await _clubRepository.GetAllAsync();
        }

        public async Task<Club?> GetClubByIdAsync(int id)
        {
            return await _clubRepository.GetByIdAsync(id);
        }

        public async Task<Club?> GetClubForDeleteAsync(int id)
        {
            return await _clubRepository.GetByIdAsync(id);
        }

        public async Task<EditClubViewModel?> GetClubForEditAsync(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return null;
            }

            var clubViewModel = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = club.AddressId,
                Address = club.Address,
                URL = club.Image,
                ClubCategory = club.ClubCategory
            };

            return clubViewModel;
        }

        public async Task<bool> UpdateClubAsync(int id, EditClubViewModel clubViewModel)
        {
            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);
            if (userClub == null)
            {
                return false;
            }

            if (clubViewModel.Image != null)
            {
                var photoResult = await _photoService.AddPhotoAsync(clubViewModel.Image);
                if (photoResult.Error != null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(userClub.Image))
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }

                var club = new Club
                {
                    Id = id,
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = clubViewModel.AddressId,
                    Address = clubViewModel.Address,
                    ClubCategory = clubViewModel.ClubCategory
                };

                return await _clubRepository.UpdateAsync(club);
            }
            else
            {
                var club = new Club
                {
                    Id = id,
                    Title = clubViewModel.Title,
                    Description = clubViewModel.Description,
                    AddressId = clubViewModel.AddressId,
                    Address = clubViewModel.Address,
                };

                return await _clubRepository.UpdateAsync(club);
            }
        }
    }
}
