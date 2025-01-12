using EP1.Models;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public enum UserRole
    {
        Admin = 0,
        Teacher = 1,
        Student = 2
    }
    public class AccountViewModel
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        // Lớp học của người dùng
        [StringLength(100, ErrorMessage = "Class cannot be longer than 100 characters.")]
        public string? Class { get; set; }
        // Chuyên ngành của người dùng
        [StringLength(100, ErrorMessage = "Specification cannot be longer than 100 characters.")]
        public string? Specification { get; set; }
        // Phân khoa của người dùng
        [StringLength(100, ErrorMessage = "Section cannot be longer than 100 characters.")]
        public string? Section { get; set; }
        // Trạng thái người dùng (hoạt động hay không)
        public bool IsActive { get; set; }
        // Vai trò người dùng (Admin, Teacher, Student)
        [Required(ErrorMessage = "Role is required.")]
        public UserRole Role { get; set; } // Role dưới dạng Enum
    }
}
