namespace EP1.Models
{
	public class SurveySubmission
	{
		public int SurveySubmissionId { get; set; }  // ID bài khảo sát
		public int SurveyId { get; set; }  // ID khảo sát
		public string UserName { get; set; }  // UserName của người dùng (liên kết với IdentityUser)
		public DateTime SubmissionDate { get; set; }  // Ngày nộp bài khảo sát
		public double TotalScore { get; set; }  // Tổng điểm bài khảo sát
		public bool Active { get; set; } = false;  // Trạng thái đang chấm điểm hay chưa

		public virtual ICollection<Answer> Answers { get; set; }  // Các câu trả lời của người dùng
		public virtual Survey Survey { get; set; }  // Liên kết với khảo sát
	}
}
