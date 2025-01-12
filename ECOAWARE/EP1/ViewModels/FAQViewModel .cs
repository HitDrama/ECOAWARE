using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class FAQViewModel
	{
		[Required]
		public int Id { get; set; } // ID của FAQ

		[Required(ErrorMessage = "Title is required")]
		[StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
		public string Title { get; set; } // Tiêu đề câu hỏi

		[Required(ErrorMessage = "Description is required")]
		public string Description { get; set; } // Mô tả câu hỏi

		public DateTime CreatedDate { get; set; } // Ngày tạo

		public string Image { get; set; } // Không có [Required]
		public IFormFile? ImgFile { get; set; } // File upload, không bắt buộc

	}
}
