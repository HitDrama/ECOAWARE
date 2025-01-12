using EP1.Helper;
using EP1.Models;
using EP1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EP1.Controllers
{
	public class AnswerController : Controller
	{
		private readonly ShopContext _context;

		public AnswerController(ShopContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int surveyId)
		{
			// Lấy thông tin khảo sát
			var survey = await _context.Surveys
				.Include(s => s.Questions)
				.FirstOrDefaultAsync(s => s.SurveyId == surveyId);

			if (survey == null)
			{
				TempData["ErrorMessage"] = "Survey does not exist.";
				return RedirectToAction("Index", "DetailSurvey", new { id = surveyId });
			}

			// Chuyển đổi sang ViewModel
			var questionViewModels = survey.Questions.Select(q => new QuestionViewModel
			{
				QuestionId = q.QuestionId,
				SurveyId = q.SurveyId,
				Content = q.Content,
				Type = q.Type,
				CorrectAnswer = q.Type == QuestionType.MultipleChoice ? q.CorrectAnswer : null,
				Points = q.Points
			}).ToList();

			ViewBag.SurveyTitle = survey.Title; // Truyền tiêu đề khảo sát vào View
			ViewBag.SurveyId = survey.SurveyId; // Truyền SurveyId vào View
			return View(questionViewModels);
		}


		[HttpPost]
		public async Task<IActionResult> SubmitAnswers(int surveyId, List<AnswerViewModel> answers)
		{
			var survey = await _context.Surveys
				.Include(s => s.Questions)
				.FirstOrDefaultAsync(s => s.SurveyId == surveyId);

			if (survey == null)
			{
				TempData["ErrorMessage"] = "Survey does not exist.";
				return RedirectToAction("Index", "DetailSurvey", new { id = surveyId });
			}

			if (answers == null || !answers.Any())
			{
				TempData["ErrorMessage"] = "No replies have been sent.";
				return RedirectToAction("Index", "DetailSurvey", new { id = surveyId });
			}

			var userId = User.Identity.Name;
			var existingSubmission = await _context.SurveySubmissions
				.FirstOrDefaultAsync(s => s.SurveyId == surveyId && s.UserName == userId);

			if (existingSubmission != null)
			{
				TempData["ErrorMessage"] = "You may only submit once to this survey.";
				return RedirectToAction("Index", "DetailSurvey", new { id = surveyId });
			}

			var submission = new SurveySubmission
			{
				SurveyId = surveyId,
				UserName = userId,
				SubmissionDate = DateTime.Now,
				TotalScore = 0
			};

			if (!ModelState.IsValid)
			{
				// Trả về view với thông báo lỗi nếu validation không thành công
				TempData["ErrorMessage"] = "Please correct the errors and submit again.";
				return View(answers);
			}

			_context.SurveySubmissions.Add(submission);
			await _context.SaveChangesAsync();

			double totalScore = 0;

			foreach (var answer in answers)
			{
				var question = survey.Questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);

				if (question != null)
				{
					// Gọi hàm chấm điểm từ GradingHelper
					var (isCorrect, score) = GradingHelper.GradeAnswer(question, answer.AnswerText);

					var answerEntity = new Answer
					{
						QuestionId = answer.QuestionId,
						AnswerText = answer.AnswerText,
						SurveySubmissionId = submission.SurveySubmissionId,
						Score = score,
						IsCorrect = isCorrect
					};

					totalScore += score;

					_context.Answers.Add(answerEntity);
				}
			}

			submission.TotalScore = totalScore;
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = $"Your work has been recorded.";
			return RedirectToAction("Index", "DetailSurvey", new { id = surveyId });
		}






	}
}
