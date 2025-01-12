using EP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EP1.Areas.System.Controllers
{
	[Area("System")]
	[Route("/System/manager")]
	[Authorize(Roles = "Admin")]
	public class ManagerController : Controller
	{
		private readonly ShopContext _context;
		private readonly UserManager<Account> _userManager;

		public ManagerController(ShopContext context, UserManager<Account> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[Route("index",Name ="Manager")]
		public async Task<IActionResult> Index()
		{
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var FAQCount = _context.FAQs.Count();
			var surveyCount = _context.Surveys.Count();

			// Số lượng người tham gia khảo sát hôm nay
			var today = DateTime.UtcNow.Date;
			var surveyTodayCount = await _context.SurveySubmissions
				.Where(s => s.SubmissionDate.Date == today)
				.CountAsync();

			// Số lượng người tham gia khảo sát trong 3 ngày gần nhất
			var lastThreeDays = DateTime.UtcNow.AddDays(-3).Date;
			var surveyLastThreeDaysCount = await _context.SurveySubmissions
				.Where(s => s.SubmissionDate.Date >= lastThreeDays)
				.CountAsync();

			// Số lượng câu hỏi hôm nay
			var questionTodayCount = await _context.UserQuestions
				.Where(q => q.CreatedDate.Date == today)
				.CountAsync();

			// Số lượng câu hỏi trong 3 ngày gần nhất
			var questionLastThreeDaysCount = await _context.UserQuestions
				.Where(q => q.CreatedDate.Date >= lastThreeDays)
				.CountAsync();

			ViewBag.SurveyTodayCount = surveyTodayCount;
			ViewBag.SurveyLastThreeDaysCount = surveyLastThreeDaysCount;
			ViewBag.QuestionTodayCount = questionTodayCount;
			ViewBag.QuestionLastThreeDaysCount = questionLastThreeDaysCount;

			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.FAQCount = FAQCount;
			ViewBag.SurveyCount = surveyCount;

			return View();
		}
	}
}
