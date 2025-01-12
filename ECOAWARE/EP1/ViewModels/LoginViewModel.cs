using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 32 characters.")]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}