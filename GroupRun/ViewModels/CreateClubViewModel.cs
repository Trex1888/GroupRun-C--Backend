using GroupRun.Data.Enum;
using GroupRun.Models;
using System.ComponentModel.DataAnnotations;

namespace GroupRun.ViewModels
{
    public class CreateClubViewModel
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public Address Address { get; set; } = new Address();

        [Required]
        public IFormFile? Image { get; set; }

        [Required]
        public ClubCategory ClubCategory { get; set; }

        public string? AppUserId { get; set; }
    }
}
