using System.ComponentModel.DataAnnotations;

namespace GroupRun.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Valid email is required")]
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; } = "";
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = "";
	}
}
