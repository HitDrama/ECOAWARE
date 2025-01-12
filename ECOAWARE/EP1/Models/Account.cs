using EP1.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace EP1.Models
{
    public class Account : IdentityUser
    {
        // Các trường tùy chỉnh
        public string? FullName { get; set; }
        public string? Class { get; set; }
        public string? Specification { get; set; }
        public string? Section { get; set; }
        public bool IsActive { get; set; }
        public bool RememberMe { get; set; }

        // Thêm trường Role
        public int Role { get; set; }
    }
}
