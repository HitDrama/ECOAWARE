using EP1.Helper;
using EP1.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EP1.ViewModels
{
	public class QuestionViewModel
	{
		public int QuestionId { get; set; }

		[Required(ErrorMessage = "Survey ID is required.")]
		public int SurveyId { get; set; }

		[Required(ErrorMessage = "Content is required.")]
		[StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
		public string Content { get; set; }

		[Required(ErrorMessage = "Type is required.")]
		public QuestionType Type { get; set; }

		[RequiredIf("Type", QuestionType.MultipleChoice, ErrorMessage = "Correct Answer is required for Multiple Choice questions.")]
		[StringLength(255, ErrorMessage = "Correct Answer cannot exceed 255 characters.")]
		public string CorrectAnswer { get; set; }


		[Range(0, 100, ErrorMessage = "Points must be between 0 and 100.")]
		public int Points { get; set; }

		

		public List<ChoiceViewModel> Choices { get; set; } = new List<ChoiceViewModel>();
	}
}
