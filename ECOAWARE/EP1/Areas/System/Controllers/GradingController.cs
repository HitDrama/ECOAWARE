using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EP1.Models;
using EP1.ViewModels;
using EP1.Helper;

namespace EP1.Areas.System.Controllers
{
	[Area("System")]
	[Route("/System/grading")]
	public class GradingController : Controller
	{
		private readonly ShopContext _context;

		public GradingController(ShopContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			// Lấy danh sách các khảo sát đã kết thúc
			var surveys = _context.Surveys
				.Where(s => s.EndDate < DateTime.Now) // Lọc theo ngày kết thúc
				.Select(s => new SurveyViewModel
				{
					SurveyId = s.SurveyId,
					Title = s.Title,
					Description = s.Description,
					StartDate = s.StartDate,
					EndDate = s.EndDate,
					
				})
				.ToList();

			return View(surveys);
		}

		[HttpGet("Grade/{surveyId}")]
		public IActionResult Grade(int surveyId)
		{
			// Lấy danh sách bài nộp cho khảo sát với SurveyId
			var submissions = _context.SurveySubmissions
				.Where(ss => ss.SurveyId == surveyId)
				.Select(ss => new SurveySubmissionViewModel
				{
					SurveySubmissionId = ss.SurveySubmissionId,
					UserName = ss.UserName,
					SubmissionDate = ss.SubmissionDate,
					TotalScore = ss.TotalScore,
					Active = ss.Active // Trạng thái chấm điểm
				})
				.ToList();

			ViewBag.SurveyId = surveyId; // Truyền SurveyId để dùng lại trong View
			return View(submissions);
		}


		[HttpGet("GradeSubmission/{submissionId}")]
		public IActionResult GradeSubmission(int submissionId)
		{
			// Lấy bài nộp từ cơ sở dữ liệu
			var submission = _context.SurveySubmissions
				.Include(ss => ss.Answers)
				.ThenInclude(a => a.Question)
				.FirstOrDefault(ss => ss.SurveySubmissionId == submissionId);

			if (submission == null)
			{
				TempData["ErrorMessage"] = "Bài nộp không tồn tại.";
				return RedirectToAction("Index", "Grading");
			}

			// Lọc câu trả lời tự luận
			var essayAnswers = submission.Answers
				.Where(a => a.Question.Type == QuestionType.Essay)
				.Select(a => new EssayAnswerViewModel
				{
					AnswerId = a.AnswerId,
					QuestionContent = a.Question.Content,
					AnswerText = a.AnswerText,
					Score = a.Score,
					IsCorrect = a.IsCorrect,
					Points = a.Question.Points // Lấy điểm tối đa từ câu hỏi
				})
				.ToList();

			// Tạo ViewModel truyền vào View
			var viewModel = new GradeSubmissionViewModel
			{
				SubmissionId = submission.SurveySubmissionId,
				UserName = submission.UserName,
				SubmissionDate = submission.SubmissionDate,
				Answers = essayAnswers
			};

			return View(viewModel);
		}


		[HttpPost]
		public async Task<IActionResult> SubmitGrading(GradeSubmissionViewModel viewModel)
		{
			Console.WriteLine($"SubmissionId: {viewModel.SubmissionId}");

			if (viewModel.Answers == null || !viewModel.Answers.Any())
			{
				TempData["ErrorMessage"] = "Không có câu trả lời nào được gửi từ View.";
				return RedirectToAction("Grade", new { surveyId = viewModel.SubmissionId });
			}

			var submission = await _context.SurveySubmissions
				.Include(ss => ss.Answers)
				.ThenInclude(a => a.Question)
				.FirstOrDefaultAsync(ss => ss.SurveySubmissionId == viewModel.SubmissionId);

			if (submission == null)
			{
				TempData["ErrorMessage"] = "Bài nộp không tồn tại.";
				return RedirectToAction("Grade", new { surveyId = submission.SurveyId });
			}

			foreach (var answerViewModel in viewModel.Answers)
			{
				var answer = submission.Answers.FirstOrDefault(a => a.AnswerId == answerViewModel.AnswerId);
				if (answer != null)
				{
					Console.WriteLine($"Before Update - AnswerId: {answer.AnswerId}, IsCorrect: {answer.IsCorrect}, Score: {answer.Score}");

					answer.IsCorrect = answerViewModel.IsCorrect;
					answer.Score = answerViewModel.IsCorrect ? answerViewModel.Score : 0;

					Console.WriteLine($"After Update - AnswerId: {answer.AnswerId}, IsCorrect: {answer.IsCorrect}, Score: {answer.Score}");
				}
			}

			// Cập nhật tổng điểm
			submission.TotalScore = submission.Answers.Sum(a => a.Score);
			Console.WriteLine($"Updated TotalScore: {submission.TotalScore}");

			// Đánh dấu trạng thái chấm điểm
			submission.Active = true;
			Console.WriteLine($"Updated Active Status: {submission.Active}");

			await _context.SaveChangesAsync();
			Console.WriteLine("Lưu thay đổi thành công.");

			TempData["SuccessMessage"] = "Chấm điểm thành công.";
			return RedirectToAction("Grade", new { surveyId = submission.SurveyId });
		}









	}
}
