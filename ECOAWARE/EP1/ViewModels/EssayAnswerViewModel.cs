namespace EP1.ViewModels
{
	public class EssayAnswerViewModel
	{
		public int AnswerId { get; set; }
		public string QuestionContent { get; set; }
		public string AnswerText { get; set; }
		public double Score { get; set; }
		public bool IsCorrect { get; set; }
		public int Points { get; set; }
	}
}
