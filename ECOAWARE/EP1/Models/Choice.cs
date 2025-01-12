namespace EP1.Models
{
	public class Choice
	{
		public int ChoiceId { get; set; }  // ID của lựa chọn
		public int QuestionId { get; set; }  // ID câu hỏi mà lựa chọn này thuộc về
		public string Content { get; set; }  // Nội dung của lựa chọn (A, B, C, D)
		public bool IsCorrect { get; set; }  // Đáp án đúng hay sai (chỉ áp dụng cho câu trắc nghiệm)

		public virtual Question Question { get; set; }  // Liên kết với câu hỏi
	}
}
