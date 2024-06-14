using GroupRun.Data.Enum;
using GroupRun.Models;
using System.ComponentModel.DataAnnotations;

namespace GroupRun.ViewModels
{
    public class EditRaceViewModel
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? URL { get; set; }

        public int AddressId { get; set; }

        [Required]
        public Address? Address { get; set; } = new Address();

        public IFormFile? Image { get; set; }

        public string? CurrentImage { get; set; }

        [Required]
        public RaceCategory RaceCategory { get; set; }
    }
}
