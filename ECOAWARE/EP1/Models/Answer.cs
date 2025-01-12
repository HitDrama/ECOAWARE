using EP1.Models;

public class Answer
{
	public int AnswerId { get; set; }  // ID của câu trả lời
	public int QuestionId { get; set; }  // ID câu hỏi mà sinh viên trả lời
	public string AnswerText { get; set; }  // Câu trả lời của sinh viên (có thể là lựa chọn hoặc văn bản tự luận)
	public double Score { get; set; }  // Điểm cho câu trả lời (do giảng viên chấm hoặc tự động chấm cho câu trắc nghiệm)
	public bool IsCorrect { get; set; }  // Trạng thái đúng/sai (chỉ áp dụng cho câu trắc nghiệm)

	// Liên kết với bảng SurveySubmission
	public int SurveySubmissionId { get; set; }
	public virtual SurveySubmission SurveySubmission { get; set; }  // Navigation property

	public virtual Question Question { get; set; }  // Liên kết với câu hỏi
}
