namespace EP1.ViewModels
{
	public class GradeSubmissionViewModel
	{
		public int SubmissionId { get; set; }
		public string UserName { get; set; }
		public DateTime SubmissionDate { get; set; }
		public List<EssayAnswerViewModel> Answers { get; set; }
	}
}
