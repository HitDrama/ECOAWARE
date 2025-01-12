using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class SurveyViewModel
	{
		public int SurveyId { get; set; } // ID khảo sát (không cần validate vì không nhập từ người dùng)

		[Required(ErrorMessage = "Title is required.")]
		[StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
		public string Title { get; set; }  // Tiêu đề khảo sát

		[Required(ErrorMessage = "Description is required.")]
		[StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
		public string Description { get; set; }  // Mô tả khảo sát

		[Required(ErrorMessage = "Start Date is required.")]
		public DateTime StartDate { get; set; }  // Ngày bắt đầu khảo sát

		[Required(ErrorMessage = "End Date is required.")]
		[CompareDate("StartDate", ErrorMessage = "End Date must be later than Start Date.")]
		public DateTime EndDate { get; set; }  // Ngày kết thúc khảo sát

		// Tính toán trạng thái dựa trên thời gian hiện tại
		public string Status
		{
			get
			{
				var now = DateTime.Now;
				if (now < StartDate)
					return "Chưa bắt đầu";
				if (now >= StartDate && now <= EndDate)
					return "Đang diễn ra";
				return "Đã kết thúc";
			}
		}

		[Required(ErrorMessage = "Allowed Roles are required.")]
		public string AllowedRoles { get; set; }  // Vai trò được phép tham gia

	


		// File upload không bắt buộc
		public string Img { get; set; } // File upload mới
		public IFormFile? ImgFile { get; set; } // Đường dẫn ảnh hiện tại


		public ICollection<QuestionViewModel>? Questions { get; set; } // Không bắt buộc
	}
}
