using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public class ProfileViewModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        // Class (optional, tối đa 100 ký tự)
        [StringLength(100, ErrorMessage = "Class cannot be longer than 100 characters.")]
        public string? Class { get; set; }
        // Specification (optional, tối đa 100 ký tự)
        [StringLength(100, ErrorMessage = "Specification cannot be longer than 100 characters.")]
        public string? Specification { get; set; }
        // Section (optional, tối đa 100 ký tự)
        [StringLength(100, ErrorMessage = "Section cannot be longer than 100 characters.")]
        public string? Section { get; set; }
    }
}
