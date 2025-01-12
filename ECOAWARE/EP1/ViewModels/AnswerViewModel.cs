using EP1.Models;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class AnswerViewModel
	{
		public int AnswerId { get; set; } // ID câu trả lời

		[Required(ErrorMessage = "Question ID is required.")]
		public int QuestionId { get; set; }  // ID câu hỏi mà câu trả lời thuộc về

		[Required(ErrorMessage = "Answer Text is required.")]
		[StringLength(2000, ErrorMessage = "Answer Text cannot exceed 2000 characters.")]
		public string AnswerText { get; set; }  // Câu trả lời của người dùng

		public double Score { get; set; }  // Điểm cho câu trả lời (mặc định là 0 cho tự luận)

		public bool IsCorrect { get; set; }  // Đúng/sai (chỉ áp dụng cho trắc nghiệm)

		public QuestionType QuestionType { get; set; }  // Loại câu hỏi (Trắc nghiệm hoặc Tự luận)

		public string CorrectAnswer { get; set; }  // Đáp án đúng (chỉ áp dụng cho trắc nghiệm)
	}
}
