namespace EP1.Models
{
	public class UserQuestion
	{
		public int Id { get; set; } // ID của câu hỏi người dùng

		public string Question { get; set; } // Nội dung câu hỏi

		public DateTime CreatedDate { get; set; } // Ngày đặt câu hỏi

		public string UserId { get; set; } // ID của người dùng từ Identity

		public int FAQId { get; set; } // ID của FAQ mà câu hỏi liên kết

		// Navigation property (tùy chọn, giúp truy cập thông tin FAQ dễ dàng)
		public FAQ FAQ { get; set; }

		public DateTime? ModifiedDate { get; set; } 
	}
}
