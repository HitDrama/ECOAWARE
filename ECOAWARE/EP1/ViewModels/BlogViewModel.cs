using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
    public class BlogViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "The title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The description is required.")]
        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "An image is required.")]
        public string Image { get; set; } // Không có [Required]
        public IFormFile? ImgFile { get; set; } // File upload, không bắt buộc
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo
    }
}
