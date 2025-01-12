namespace EP1.Models
{
	public class Survey
	{
		public int SurveyId { get; set; }  // ID của khảo sát
		public string Title { get; set; }  // Tiêu đề của khảo sát
		public string Description { get; set; }  // Mô tả chi tiết khảo sát
		public DateTime StartDate { get; set; }  // Ngày bắt đầu khảo sát
		public DateTime EndDate { get; set; }  // Ngày kết thúc khảo sát
		public string Status { get; set; }  // Trạng thái của khảo sát (Chưa bắt đầu, Đang diễn ra, Đã kết thúc)

		public string AllowedRoles { get; set; }  // Danh sách các vai trò được phép tham gia (dạng chuỗi, ví dụ: "Student,Teacher")

		public string Img { get; set; }  // Đường dẫn hoặc URL tới hình ảnh đại diện của khảo sát

		public virtual ICollection<Question> Questions { get; set; }  // Các câu hỏi trong khảo sát
		public virtual ICollection<SurveySubmission> SurveySubmissions { get; set; }  // Các bài nộp của người dùng
	}
}
