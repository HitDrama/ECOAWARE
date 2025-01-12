namespace EP1.Models
{
	public class FAQ
	{
		public int Id { get; set; } // ID của FAQ

		
		public string Title { get; set; } // Tiêu đề câu hỏi

		public string Description { get; set; } // Mô tả câu hỏi

		public DateTime CreatedDate { get; set; } // Ngày tạo

		public string Image { get; set; } // Hình ảnh upload

	
	}
}
