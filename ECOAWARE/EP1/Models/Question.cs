namespace EP1.Models
{
	public class Question
	{
		public int QuestionId { get; set; }  // ID câu hỏi
		public int SurveyId { get; set; }  // ID khảo sát mà câu hỏi thuộc về
		public string Content { get; set; }  // Nội dung câu hỏi
		public QuestionType Type { get; set; }  // Loại câu hỏi (Trắc nghiệm hoặc Tự luận)
		public string CorrectAnswer { get; set; }  // Đáp án đúng (chỉ dùng cho câu hỏi trắc nghiệm)
		public int Points { get; set; }  // Điểm cho câu hỏi (Dùng cho cả trắc nghiệm và tự luận)

		

		public virtual ICollection<Choice> Choices { get; set; }  // Các lựa chọn cho câu hỏi trắc nghiệm (nếu có)
		public virtual Survey Survey { get; set; }  // Liên kết với khảo sát
	}
}
