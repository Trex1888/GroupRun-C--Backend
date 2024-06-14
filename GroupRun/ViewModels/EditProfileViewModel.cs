using System.ComponentModel.DataAnnotations;

namespace GroupRun.ViewModels
{
    public class EditProfileViewModel
    {
        public string? Id { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string? UserName { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        [Range(1, 100)]
        public int? Pace { get; set; }

        [Range(0, 10000)]
        public int? Mileage { get; set; }
        public string? Description { get; set; }

        public string? ProfileImageUrl { get; set; }

        public IFormFile? Image { get; set; }
    }
}
