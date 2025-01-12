using Microsoft.AspNetCore.Mvc;
using EP1.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EP1.ViewModels;
using System.IO;
using IOFile = System.IO.File;

namespace EP1.Areas.System.Controllers
{
	[Area("System")]
	[Route("/System/survey")]
	[Authorize(Roles = "Admin")]
	public class SurveyController : Controller
	{
		private readonly ShopContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public SurveyController(ShopContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		// GET: /Survey/
		public async Task<IActionResult> Index()
		{
			try
			{
				var surveys = await _context.Surveys.ToListAsync();
				return View(surveys);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				TempData["ErrorMessage"] = "Unable to fetch surveys. Please try again later.";
				return View(new List<Survey>()); // Trả về danh sách rỗng nếu có lỗi
			}
		}

		// GET: /Survey/Create
		[Route("add")]
		public IActionResult Create()
		{
			return View(new SurveyViewModel
			{
				Questions = new List<QuestionViewModel>()
			});
		}

		[HttpPost("add")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SurveyViewModel model, IFormFile imgFile)
		{
			// Loại bỏ lỗi liên quan đến trường Img
			ModelState.Remove("Img");

			if (ModelState.IsValid)
			{
				// Xử lý upload ảnh
				string imagePath = string.Empty;
				if (imgFile != null)
				{
					try
					{
						// Lấy tên file và lưu vào thư mục wwwroot/images/Survey
						string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
						string extension = Path.GetExtension(imgFile.FileName);
						imagePath = fileName + "_" + Guid.NewGuid().ToString() + extension;
						string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Survey", imagePath);

						// Đảm bảo thư mục tồn tại
						Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images/Survey"));

						// Lưu ảnh vào thư mục
						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await imgFile.CopyToAsync(stream);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"File Upload Error: {ex.Message}");
						TempData["ErrorMessage"] = "An error occurred while uploading the image.";
						return View(model);
					}
				}

				// Tạo đối tượng Survey từ ViewModel
				var survey = new Survey
				{
					Title = model.Title,
					Description = model.Description,
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					AllowedRoles = model.AllowedRoles,
					Img = imagePath, // Đường dẫn ảnh
					Status = DateTime.Now < model.StartDate ? "Chưa bắt đầu" :
							 DateTime.Now > model.EndDate ? "Đã kết thúc" : "Đang diễn ra"
				};

				try
				{
					// Lưu khảo sát vào cơ sở dữ liệu
					_context.Surveys.Add(survey);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Database Save Error: {ex.Message}");
					TempData["ErrorMessage"] = "An error occurred while saving the survey.";
					return View(model);
				}

				TempData["SuccessMessage"] = "Survey created successfully.";
				return RedirectToAction(nameof(Index));
			}

			// Debug lỗi nếu ModelState không hợp lệ
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
				foreach (var error in errors)
				{
					Console.WriteLine($"ModelState Error: {error}");
				}
			}

			return View(model);
		}





		[HttpGet]
		[Route("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var survey = await _context.Surveys.FindAsync(id);

				if (survey == null)
				{
					return NotFound();
				}

				// Xóa ảnh cũ nếu có
				if (!string.IsNullOrEmpty(survey.Img))
				{
					string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Survey", survey.Img);
					if (IOFile.Exists(oldImagePath))
					{
						IOFile.Delete(oldImagePath);
					}
				}

				_context.Surveys.Remove(survey);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				TempData["ErrorMessage"] = "Cannot delete this survey because it has associated questions.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		[Route("edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var survey = await _context.Surveys.FindAsync(id);

			if (survey == null)
			{
				return NotFound();
			}

			var model = new SurveyViewModel
			{
				SurveyId = survey.SurveyId,
				Title = survey.Title,
				Description = survey.Description,
				StartDate = survey.StartDate,
				EndDate = survey.EndDate,
				AllowedRoles = survey.AllowedRoles,
				Img = survey.Img // Đường dẫn ảnh
			};

			return View(model);
		}

		[HttpPost]
		[Route("edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, SurveyViewModel model, IFormFile? imgFile)
		{
			if (id != model.SurveyId)
			{
				TempData["ErrorMessage"] = "Invalid survey ID.";
				return BadRequest();
			}

			if (ModelState.IsValid)
			{
				var survey = await _context.Surveys.FindAsync(id);

				if (survey == null)
				{
					TempData["ErrorMessage"] = "Survey not found.";
					return NotFound();
				}

				// Xử lý upload ảnh mới
				if (imgFile != null)
				{
					try
					{
						// Xóa ảnh cũ nếu có
						if (!string.IsNullOrEmpty(survey.Img))
						{
							string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Survey", survey.Img);
							if (IOFile.Exists(oldImagePath))
							{
								IOFile.Delete(oldImagePath);
							}
						}

						// Lưu ảnh mới
						string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
						string extension = Path.GetExtension(imgFile.FileName);
						string newImagePath = fileName + "_" + Guid.NewGuid().ToString() + extension;
						string newFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Survey", newImagePath);

						Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "images/Survey"));

						using (var stream = new FileStream(newFilePath, FileMode.Create))
						{
							await imgFile.CopyToAsync(stream);
						}

						survey.Img = newImagePath;
					}
					catch (Exception ex)
					{
						Console.WriteLine($"File Upload Error: {ex.Message}");
						TempData["ErrorMessage"] = "An error occurred while uploading the image.";
						return View(model);
					}
				}
				else
				{
					// Nếu không tải ảnh mới, giữ nguyên ảnh cũ từ ViewModel
					survey.Img = model.Img;
				}

				// Cập nhật thông tin khác
				survey.Title = model.Title;
				survey.Description = model.Description;
				survey.StartDate = model.StartDate;
				survey.EndDate = model.EndDate;
				survey.AllowedRoles = model.AllowedRoles;

				// Cập nhật trạng thái dựa trên thời gian
				survey.Status = DateTime.Now < survey.StartDate
					? "Chưa bắt đầu"
					: DateTime.Now > survey.EndDate
						? "Đã kết thúc"
						: "Đang diễn ra";

				try
				{
					_context.Surveys.Update(survey);
					await _context.SaveChangesAsync();
					TempData["SuccessMessage"] = "Survey updated successfully.";
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Database Save Error: {ex.Message}");
					TempData["ErrorMessage"] = "An error occurred while updating the survey.";
					return View(model);
				}

				return RedirectToAction(nameof(Index));
			}

			var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
			foreach (var error in errors)
			{
				Console.WriteLine($"ModelState Error: {error}");
			}

			return View(model);
		}





	}
}
