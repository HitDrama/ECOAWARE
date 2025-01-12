using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class UserQuestionViewModel
	{
		[Required]
		public int Id { get; set; } // ID của câu hỏi người dùng

		[Required(ErrorMessage = "Câu hỏi không được để trống")]
		[StringLength(1000, ErrorMessage = "Câu hỏi không được vượt quá 1000 ký tự")]
		public string Question { get; set; } // Nội dung câu hỏi

		[Required]
		public DateTime CreatedDate { get; set; } // Ngày đặt câu hỏi

		[Required]
		public string UserId { get; set; } // ID của người dùng từ Identity

		[Required]
		public int FAQId { get; set; } // ID của FAQ mà câu hỏi liên kết
	}
}
