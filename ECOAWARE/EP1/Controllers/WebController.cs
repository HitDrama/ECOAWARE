using EP1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EP1.Extensions;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EP1.ViewModels;
using EP1.Services;
using Microsoft.Identity.Client;
using System.Xml.Linq;

namespace EP1.Controllers
{
	public class WebController : Controller

	{
		private readonly ShopContext db;
		private readonly UserManager<Account> _userManager;
		public WebController(ShopContext context, UserManager<Account> userManager)
		{
			db = context;
			_userManager = userManager;
		}

		[Route("/", Name = "trangchu")]
		public async Task<IActionResult> Index(string category = "All", int page = 1, int pageSize = 4)
		{
			IEnumerable<Survey> surveys;
			int totalSurveys;

			// Chọn danh sách theo danh mục
			switch (category)
			{
				case "Ongoing":
					surveys = db.Surveys.Where(s => s.Status == "Đang diễn ra");
					break;
				case "Upcoming":
					surveys = db.Surveys.Where(s => s.Status == "Chưa bắt đầu");
					break;
				default:
					surveys = db.Surveys;
					break;
			}

			totalSurveys = surveys.Count();

			// Phân trang
			var pagedSurveys = surveys
				.OrderBy(s => s.SurveyId)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			int totalPages = (int)Math.Ceiling(totalSurveys / (double)pageSize);

			// Đếm số lượng tài khoản theo Role
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var FAQCount = db.FAQs.Count();

			// Truyền dữ liệu qua ViewBag
			ViewBag.Surveys = pagedSurveys;
			ViewBag.TotalSurveys = totalSurveys;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;
			ViewBag.CurrentCategory = category;

			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.FAQCount = FAQCount;

			return View();
		}

		[Route("/FAQ", Name = "FAQ")]
		public async Task<IActionResult> FAQ(int page = 1, int pageSize = 5)
		{
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var FAQCount = db.FAQs.Count();
			var surveyCount = db.Surveys.Count();

			var faqs = db.FAQs
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(f => new FAQ
				{
					Id = f.Id,
					Title = f.Title,
					Image = f.Image,
					CreatedDate = f.CreatedDate,
					Description = f.Description.Length > 200
						? f.Description.Substring(0, 200) + "..."
						: f.Description
				}).ToList();

			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.FAQCount = FAQCount;
			ViewBag.SurveyCount = surveyCount;

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = (int)Math.Ceiling((double)db.FAQs.Count() / pageSize);
			return View(faqs);
		}

		[Route("/FAQ/{id}", Name = "DetailFAQ")]
		public async Task<IActionResult> DetailFAQ(int id)
		{
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var FAQCount = db.FAQs.Count();
			var surveyCount = db.Surveys.Count();
			// Lấy FAQ theo ID
			var faq = await db.FAQs.FirstOrDefaultAsync(f => f.Id == id);

			if (faq == null)
			{
				return NotFound();
			}

			// Lấy danh sách comment liên kết với FAQ này
			var comments = await db.UserQuestions
				.Where(c => c.FAQId == id)
				.Select(c => new
				{
					c.Id,
					c.Question,
					c.CreatedDate,
					c.UserId, // Bao gồm UserId để sử dụng trong View
					UserName = db.Users.FirstOrDefault(u => u.Id == c.UserId).FullName ?? "Unknown"
				})
				.OrderByDescending(c => c.CreatedDate)
				.ToListAsync();


			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.FAQCount = FAQCount;
			ViewBag.SurveyCount = surveyCount;
			// Gắn comment vào ViewBag
			ViewBag.Comments = comments;

			// Truyền dữ liệu qua View
			return View(faq);
		}

		[HttpPost]
		[Route("/FAQ/AddComment", Name = "AddComment")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddComment(int faqId, string commentText)
		{
			if (!User.Identity.IsAuthenticated)
			{
				TempData["ErrorMessage"] = "You must be logged in to comment.";
				return RedirectToAction("DetailFAQ", new { id = faqId });
			}

			// Log giá trị nhận được để kiểm tra
			Console.WriteLine($"FAQId: {faqId}, CommentText: {commentText}");

			if (string.IsNullOrWhiteSpace(commentText))
			{
				TempData["ErrorMessage"] = "Comment cannot be empty.";
				return RedirectToAction("DetailFAQ", new { id = faqId });
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var newComment = new UserQuestion
			{
				FAQId = faqId,
				Question = commentText,
				CreatedDate = DateTime.Now,
				UserId = userId
			};

			try
			{
				db.UserQuestions.Add(newComment);
				await db.SaveChangesAsync();
				TempData["SuccessMessage"] = "Comment added successfully!";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding comment: {ex.Message}");
				TempData["ErrorMessage"] = "An error occurred while adding the comment.";
			}

			return RedirectToAction("DetailFAQ", new { id = faqId });
		}



		[HttpPost]
		[Route("/Web/DeleteComment", Name = "DeleteComment")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteComment(int id)
		{
			if (!User.Identity.IsAuthenticated)
			{
				TempData["ErrorMessage"] = "You must be logged in to delete comments.";
				return RedirectToAction("FAQ");
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var comment = await db.UserQuestions.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

			if (comment == null)
			{
				TempData["ErrorMessage"] = "You can only delete your own comments.";
				return RedirectToAction("FAQ");
			}

			db.UserQuestions.Remove(comment);
			await db.SaveChangesAsync();

			TempData["SuccessMessage"] = "Comment deleted successfully!";
			return RedirectToAction("DetailFAQ", new { id = comment.FAQId });
		}




		[HttpGet]
		[Route("/FAQ/EditComment/{id}", Name = "EditComment")]
		public async Task<IActionResult> EditComment(int id)
		{
			// Kiểm tra người dùng đã đăng nhập
			if (!User.Identity.IsAuthenticated)
			{
				TempData["ErrorMessage"] = "You must be logged in to edit comments.";
				return RedirectToAction("FAQ");
			}

			// Lấy thông tin bình luận từ cơ sở dữ liệu
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var comment = await db.UserQuestions.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

			if (comment == null)
			{
				TempData["ErrorMessage"] = "You can only edit your own comments.";
				return RedirectToAction("FAQ");
			}

			var viewModel = new UserQuestionViewModel
			{
				Id = comment.Id,
				Question = comment.Question,
				CreatedDate = comment.CreatedDate,
				UserId = comment.UserId,
				FAQId = comment.FAQId
			};

			return View(viewModel); // Trả về view form chỉnh sửa
		}

		[HttpPost]
		[Route("/FAQ/EditComment/{id}", Name = "EditComment")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditComment(UserQuestionViewModel model)
		{
			// Kiểm tra người dùng đã đăng nhập
			if (!User.Identity.IsAuthenticated)
			{
				TempData["ErrorMessage"] = "You must be logged in to edit comments.";
				return RedirectToAction("FAQ");
			}

			// Kiểm tra tính hợp lệ của model
			if (!ModelState.IsValid)
			{
				TempData["ErrorMessage"] = "Invalid input. Please check your comment.";
				return View(model); // Trả về form chỉnh sửa kèm thông báo lỗi
			}

			// Lấy bình luận từ cơ sở dữ liệu để chỉnh sửa
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var comment = await db.UserQuestions.FirstOrDefaultAsync(c => c.Id == model.Id && c.UserId == userId);

			if (comment == null)
			{
				TempData["ErrorMessage"] = "You can only edit your own comments.";
				return RedirectToAction("FAQ");
			}

			// Cập nhật nội dung bình luận
			comment.Question = model.Question;
			comment.ModifiedDate = DateTime.Now;

			try
			{
				// Lưu thay đổi vào cơ sở dữ liệu
				db.UserQuestions.Update(comment);
				await db.SaveChangesAsync();

				TempData["SuccessMessage"] = "Comment updated successfully!";
				return RedirectToAction("FAQ");
			}
			catch (Exception ex)
			{
				// Log lỗi và hiển thị thông báo
				TempData["ErrorMessage"] = "An error occurred while updating the comment. Please try again.";
				Console.WriteLine($"Error updating comment: {ex.Message}");
				return View(model);
			}
		}





		[HttpGet]
		[Route("/Web/Contact", Name = "Contact")]
		public IActionResult Contact()
		{
			return View();
		}

		[HttpPost]
		[Route("/Web/Contact", Name = "Contact")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> PostContact(string name, string email, string subject, string message, [FromServices] EmailService emailService)
		{
			// Kiểm tra dữ liệu đầu vào
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
			{
				TempData["ErrorMessage"] = "All fields are required.";
				return RedirectToAction("Contact");
			}

			try
			{
				// Gửi email
				string emailMessage = $"Name: {name}\nEmail: {email}\nSubject: {subject}\nMessage: {message}";
				await emailService.SendEmailAsync("admin@example.com", subject, emailMessage);

				TempData["SuccessMessage"] = "Your message has been sent successfully!";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending email: {ex.Message}");
				TempData["ErrorMessage"] = "An error occurred while sending your message.";
			}
			Console.WriteLine($"Name: {name}, Email: {email}, Subject: {subject}, Message: {message}");
			return RedirectToAction("Contact");
		}


		[Route("/Blog", Name = "Blog")]
		public async Task<IActionResult> Blog(int page = 1, int pageSize = 5)
		{
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var BlogCount = db.Blogs.Count();
			var surveyCount = db.Surveys.Count();

			var blogs = db.Blogs
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(f => new FAQ
				{
					Id = f.Id,
					Title = f.Title,
					Image = f.Image,
					CreatedDate = f.CreatedDate,
					Description = f.Description.Length > 200
						? f.Description.Substring(0, 200) + "..."
						: f.Description
				}).ToList();

			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.BlogCount = BlogCount;
			ViewBag.SurveyCount = surveyCount;

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = (int)Math.Ceiling((double)db.Blogs.Count() / pageSize);
			return View(blogs);
		}


		[Route("/Blog/{id}", Name = "DetailBlog")]
		public async Task<IActionResult> DetailBlog(int id)
		{
			var teacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
			var studentCount = (await _userManager.GetUsersInRoleAsync("Student")).Count;
			var BlogCount = db.Blogs.Count();
			var surveyCount = db.Surveys.Count();
			// Lấy FAQ theo ID
			var blog = await db.Blogs.FirstOrDefaultAsync(f => f.Id == id);
			if (blog == null)
			{
				return NotFound();
			}

			// Truyền số lượng tài khoản qua ViewBag
			ViewBag.TeacherCount = teacherCount;
			ViewBag.StudentCount = studentCount;
			ViewBag.BlogCount = BlogCount;
			ViewBag.SurveyCount = surveyCount;
			

			// Truyền dữ liệu qua View
			return View(blog);




		}
	}

}
