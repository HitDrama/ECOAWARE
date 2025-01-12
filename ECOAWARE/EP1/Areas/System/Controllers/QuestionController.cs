using EP1.Models;
using EP1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EP1.Areas.System.Controllers
{
	[Area("System")]
	[Route("/System/questions")]
	[Authorize(Roles = "Admin")]
	public class QuestionController : Controller
	{
		private readonly ShopContext _context;

		public QuestionController(ShopContext context)
		{
			_context = context;
		}

		// GET: /System/question/{surveyId}
		[HttpGet("{surveyId}")]
		public async Task<IActionResult> Index(int surveyId)
		{
			var questions = await _context.Questions
				.Where(q => q.SurveyId == surveyId)
				.Select(q => new QuestionViewModel
				{
					QuestionId = q.QuestionId,
					SurveyId = q.SurveyId,
					Content = q.Content,
					Type = q.Type,
					CorrectAnswer = q.CorrectAnswer,
					Points = q.Points,
					Choices = q.Choices.Select(c => new ChoiceViewModel
					{
						ChoiceId = c.ChoiceId,
						QuestionId = c.QuestionId,
						Content = c.Content,
						IsCorrect = c.IsCorrect
					}).ToList()
				})
				.ToListAsync();

			ViewData["SurveyId"] = surveyId;
			return View(questions);
		}

		// GET: /System/question/{surveyId}/add
		[HttpGet("{surveyId}/add")]
		public IActionResult Create(int surveyId)
		{
			var model = new QuestionViewModel
			{
				SurveyId = surveyId,
				Choices = new List<ChoiceViewModel>() // Initialize empty choices
			};

			return View(model);
		}

		// POST: /System/question/{surveyId}/add
		[HttpPost("{surveyId}/add")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(int surveyId, QuestionViewModel model)
		{
			if (!ModelState.IsValid)
			{
				// Ghi log lỗi để kiểm tra nếu validation không hợp lệ
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					Console.WriteLine($"Validation Error: {error.ErrorMessage}");
				}

				return View(model);
			}

			// Gán giá trị mặc định cho CorrectAnswer nếu là Essay
			if (model.Type == QuestionType.Essay)
			{
				model.CorrectAnswer = "Not Applicable"; // Giá trị mặc định
			}

			var question = new Question
			{
				SurveyId = surveyId,
				Content = model.Content,
				Type = model.Type,
				Points = model.Points,
				CorrectAnswer = model.CorrectAnswer, // Gán giá trị mặc định hoặc giá trị từ Multiple Choice
				Choices = model.Type == QuestionType.MultipleChoice
					? model.Choices.Select(c => new Choice
					{
						Content = c.Content,
						IsCorrect = c.IsCorrect
					}).ToList()
					: null // Choices rỗng cho Essay
			};

			_context.Questions.Add(question);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index), new { surveyId });
		}


		// GET: /System/question/{surveyId}/delete/{id}
		[HttpGet("{surveyId}/delete/{id}")]
		public async Task<IActionResult> Delete(int surveyId, int id)
		{
			var question = await _context.Questions
				.Where(q => q.QuestionId == id && q.SurveyId == surveyId)
				.FirstOrDefaultAsync();

			if (question == null)
			{
				return NotFound();
			}

			// Thực hiện xóa câu hỏi
			_context.Questions.Remove(question);
			await _context.SaveChangesAsync();

			// Chuyển hướng về danh sách câu hỏi
			return RedirectToAction(nameof(Index), new { surveyId });
		}


		// GET: /System/question/{surveyId}/edit/{id}
		[HttpGet("{surveyId}/edit/{id}")]
		public async Task<IActionResult> Edit(int surveyId, int id)
		{
			var question = await _context.Questions
				.Include(q => q.Choices) // Load các lựa chọn nếu có
				.FirstOrDefaultAsync(q => q.QuestionId == id && q.SurveyId == surveyId);

			if (question == null)
			{
				return NotFound();
			}

			var model = new QuestionViewModel
			{
				QuestionId = question.QuestionId,
				SurveyId = question.SurveyId,
				Content = question.Content,
				Type = question.Type, // Gửi đúng giá trị Type
				Points = question.Points,
				CorrectAnswer = question.CorrectAnswer ?? string.Empty, // Giá trị mặc định
				Choices = question.Choices?.Select(c => new ChoiceViewModel
				{
					ChoiceId = c.ChoiceId,
					QuestionId = c.QuestionId,
					Content = c.Content,
					IsCorrect = c.IsCorrect
				}).ToList() ?? new List<ChoiceViewModel>()
			};

			return View(model);
		}

		// POST: /System/question/{surveyId}/edit/{id}
		[HttpPost("{surveyId}/edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int surveyId, int id, QuestionViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var question = await _context.Questions
				.Include(q => q.Choices) // Load các lựa chọn nếu có
				.FirstOrDefaultAsync(q => q.QuestionId == id && q.SurveyId == surveyId);

			if (question == null)
			{
				return NotFound();
			}

			// Cập nhật thông tin câu hỏi
			question.Content = model.Content;
			question.Type = model.Type; // Enum được ánh xạ đúng
			question.Points = model.Points;

			if (model.Type == QuestionType.MultipleChoice)
			{
				question.CorrectAnswer = model.CorrectAnswer;

				// Cập nhật các lựa chọn
				_context.Choices.RemoveRange(question.Choices);
				question.Choices = model.Choices.Select(c => new Choice
				{
					Content = c.Content,
					IsCorrect = c.IsCorrect
				}).ToList();
			}
			else
			{
				question.CorrectAnswer = "Not Applicable";
				if (question.Choices != null)
				{
					_context.Choices.RemoveRange(question.Choices);
					question.Choices = null;
				}
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index), new { surveyId });
		}



	}
}
