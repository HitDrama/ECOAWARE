using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class SurveySubmissionViewModel
	{
		public int SurveySubmissionId { get; set; } // ID bài khảo sát (không cần validate)

		[Required(ErrorMessage = "Survey ID is required.")]
		public int SurveyId { get; set; }  // ID khảo sát

		[Required(ErrorMessage = "UserName is required.")]
		public string UserName { get; set; }  // UserName của người nộp bài

		[Required(ErrorMessage = "Submission Date is required.")]
		public DateTime SubmissionDate { get; set; }  // Ngày nộp bài

		[Range(0, double.MaxValue, ErrorMessage = "Total Score must be a positive number.")]
		public double TotalScore { get; set; }  // Tổng điểm bài khảo sát

		public bool Active { get; set; }  // Trạng thái đang chấm điểm hay chưa (không cần validate)

		public ICollection<AnswerViewModel> Answers { get; set; }  // Các câu trả lời
	}
}
