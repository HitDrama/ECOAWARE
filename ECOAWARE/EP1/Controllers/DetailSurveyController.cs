using EP1.Models;
using EP1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EP1.Controllers
{
	public class DetailSurveyController : Controller
	{
		private readonly ShopContext _context; // Inject DbContext
		private readonly UserManager<Account> _userManager;

		public DetailSurveyController(ShopContext context, UserManager<Account> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// Action hiển thị danh sách các khảo sát
		[HttpGet]
		[Route("Detail/{id}")]
		public async Task<IActionResult> Index(int id)
		{
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var FAQCount = _context.FAQs.Count();
			var surveyCount = _context.Surveys.Count();

			// Tìm kiếm khảo sát theo ID
			var survey = _context.Surveys
				.Where(s => s.SurveyId == id)
				.Select(s => new SurveyViewModel
				{
					SurveyId = s.SurveyId,
					Title = s.Title,
					Description = s.Description,
					StartDate = s.StartDate,
					EndDate = s.EndDate,
					AllowedRoles = s.AllowedRoles,
					
				})
				.FirstOrDefault();

			if (survey == null)
			{
				TempData["ErrorMessage"] = "Khảo sát không tồn tại.";
				return RedirectToAction("Index");
			}
			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.FAQCount = FAQCount;
			ViewBag.SurveyCount = surveyCount;

			return View(survey);
		}

		[HttpGet]
		[Route("join/{id}")]
		[Authorize]
		public async Task<IActionResult> Join(int id)
		{
			// Tìm khảo sát
			var survey = await _context.Surveys
				.FirstOrDefaultAsync(s => s.SurveyId == id);

			if (survey == null)
			{
				TempData["ErrorMessage"] = $"Khảo sát không tồn tại. Survey ID: {id}";
				return RedirectToAction("Index", new { id });
			}

			// Kiểm tra trạng thái
			var now = DateTime.Now;
			if (now < survey.StartDate)
			{
				TempData["ErrorMessage"] = "Khảo sát chưa bắt đầu.";
				return RedirectToAction("Index", new { id });
			}
			if (now > survey.EndDate)
			{
				TempData["ErrorMessage"] = "Khảo sát đã kết thúc.";
				return RedirectToAction("Index", new { id });
			}

			// Kiểm tra quyền
			var userId = _userManager.GetUserId(User);
			var allowedRoles = survey.AllowedRoles.Split(',');
			var userRoles = await (from ur in _context.UserRoles
								   join role in _context.Roles on ur.RoleId equals role.Id
								   where ur.UserId == userId
								   select role.Name).ToListAsync();

			if (!allowedRoles.Any(role => userRoles.Contains(role)))
			{
				TempData["ErrorMessage"] = $"Bạn không có quyền tham gia khảo sát này. Vai trò của bạn: {string.Join(", ", userRoles)}";
				return RedirectToAction("Index", new { id });
			}

			// Chuyển hướng đến AnswerController với surveyId
			return RedirectToAction("Index", "Answer", new { surveyId = id });
		}




	}
}
