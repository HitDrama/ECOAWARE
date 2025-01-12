using EP1.Models;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 32 characters.")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 32 characters.")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? RePassword { get; set; }

        [Required]
        [StringLength(160, ErrorMessage = "Fullname cannot be longer than 160 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        [StringLength(20, ErrorMessage = "Class cannot be longer than 20 characters.")]
        public string? Class { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Specification cannot be longer than 50 characters.")]
        public string? Specification { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Section cannot be longer than 20 characters.")]
        public string? Section { get; set; }

        public bool RememberMe { get; set; }

        // Thêm trường Role với validation
        [Required(ErrorMessage = "Role is required.")]
        //[EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role. Valid values are Admin, Teacher, and Student.")]
        public UserRole Role { get; set; }
    }
}   
