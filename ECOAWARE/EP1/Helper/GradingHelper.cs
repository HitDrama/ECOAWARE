using EP1.Models;

namespace EP1.Helper
{
	public static class GradingHelper
	{
		/// <summary>
		/// Hàm chấm điểm cho câu trả lời.
		/// </summary>
		/// <param name="question">Câu hỏi cần chấm điểm.</param>
		/// <param name="answerText">Câu trả lời của người dùng.</param>
		/// <returns>Tuple: IsCorrect (đúng/sai), Score (điểm).</returns>
		public static (bool IsCorrect, double Score) GradeAnswer(Question question, string answerText)
		{
			if (question.Type == QuestionType.MultipleChoice)
			{
				var isCorrect = string.Equals(question.CorrectAnswer?.Trim(), answerText?.Trim(), StringComparison.OrdinalIgnoreCase);
				var score = isCorrect ? question.Points : 0;
				return (isCorrect, score);
			}
			else if (question.Type == QuestionType.Essay)
			{
				return (false, 0); // Mặc định admin sẽ chấm điểm
			}

			return (false, 0);
		}
	}
}
