using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class ChoiceViewModel
	{
		public int ChoiceId { get; set; } // ID lựa chọn (không cần validate)

		[Required(ErrorMessage = "Question ID is required.")]
		public int QuestionId { get; set; }  // ID câu hỏi mà lựa chọn thuộc về

		[Required(ErrorMessage = "Content is required.")]
		[StringLength(255, ErrorMessage = "Content cannot exceed 255 characters.")]
		public string Content { get; set; }  // Nội dung của lựa chọn

		public bool IsCorrect { get; set; }  // Đáp án đúng hay sai
	}
}
