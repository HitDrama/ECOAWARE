using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using EP1.Models;
using System;
using System.IO;
using System.Linq;
using EP1.ViewModels;
using IOFile = System.IO.File;
using Microsoft.AspNetCore.Authorization;

namespace EP1.Areas.System.Controllers
{
	[Area("System")]
	[Route("/System/FAQ")]
	[Authorize(Roles = "Admin")]
	public class FAQController : Controller
	{
		private readonly ShopContext _context;
		private readonly IWebHostEnvironment _env;


		public FAQController(ShopContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}
		public IActionResult Index()
		{
			// Lấy danh sách tất cả FAQ
			var faqs = _context.FAQs.ToList();

			return View(faqs);
		}

		[HttpGet("Add")]
		public IActionResult Add()
		{
			// Trả về view model thay vì model
			var faqViewModel = new FAQViewModel();
			return View(faqViewModel);
		}

		[HttpPost("Add")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(FAQViewModel model, IFormFile imgFile)
		{
			// Loại bỏ lỗi ModelState cho trường Image nếu có
			ModelState.Remove("Image");

			if (ModelState.IsValid)
			{
				// Xử lý upload ảnh
				if (imgFile != null)
				{
					string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
					string extension = Path.GetExtension(imgFile.FileName);
					string uniqueFileName = fileName + "_" + Guid.NewGuid().ToString() + extension;

					string filePath = Path.Combine(_env.WebRootPath, "images/FAQ", uniqueFileName);
					Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images/FAQ"));

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await imgFile.CopyToAsync(stream);
					}

					model.Image = uniqueFileName;
				}
				else
				{
					model.Image = null;
				}

				// Thêm FAQ vào cơ sở dữ liệu
				var faq = new FAQ
				{
					Title = model.Title,
					Description = model.Description,
					CreatedDate = DateTime.Now,
					Image = model.Image
				};

				_context.FAQs.Add(faq);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "FAQ created successfully.";
				return RedirectToAction("Index");
			}

			// Ghi log lỗi nếu ModelState không hợp lệ
			var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
			foreach (var error in errors)
			{
				Console.WriteLine($"ModelState Error: {error}");
			}

			return View(model);
		}

		[HttpGet]
		[Route("edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var faq = await _context.FAQs.FindAsync(id);

			if (faq == null)
			{
				return NotFound();
			}

			var model = new FAQViewModel
			{
				Id = faq.Id,
				Title = faq.Title,
				Description = faq.Description,
				Image = faq.Image // Đường dẫn ảnh hiện tại
			};

			return View(model);
		}

		[HttpPost]
		[Route("edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, FAQViewModel model, IFormFile? imgFile)
		{
			if (id != model.Id)
			{
				TempData["ErrorMessage"] = "Invalid FAQ ID.";
				return BadRequest();
			}

			// Loại bỏ lỗi cho Image nếu không upload ảnh mới
			if (imgFile == null)
			{
				ModelState.Remove("Image");
			}

			if (ModelState.IsValid)
			{
				var faq = await _context.FAQs.FindAsync(id);

				if (faq == null)
				{
					TempData["ErrorMessage"] = "FAQ not found.";
					return NotFound();
				}

				// Xử lý upload ảnh mới
				if (imgFile != null)
				{
					try
					{
						// Xóa ảnh cũ nếu có
						if (!string.IsNullOrEmpty(faq.Image))
						{
							string oldImagePath = Path.Combine(_env.WebRootPath, "images/FAQ", faq.Image);
							if (IOFile.Exists(oldImagePath))
							{
								IOFile.Delete(oldImagePath);
							}
						}

						// Lưu ảnh mới
						string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
						string extension = Path.GetExtension(imgFile.FileName);
						string newImagePath = fileName + "_" + Guid.NewGuid().ToString() + extension;
						string newFilePath = Path.Combine(_env.WebRootPath, "images/FAQ", newImagePath);

						Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images/FAQ"));

						using (var stream = new FileStream(newFilePath, FileMode.Create))
						{
							await imgFile.CopyToAsync(stream);
						}

						faq.Image = newImagePath;
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
					faq.Image = model.Image;
				}

				// Cập nhật các trường khác
				faq.Title = model.Title;
				faq.Description = model.Description;

				try
				{
					_context.FAQs.Update(faq);
					await _context.SaveChangesAsync();
					TempData["SuccessMessage"] = "FAQ updated successfully.";
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Database Save Error: {ex.Message}");
					TempData["ErrorMessage"] = "An error occurred while updating the FAQ.";
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



		[HttpGet]
		[Route("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var faq = await _context.FAQs.FindAsync(id);

				if (faq == null)
				{
					TempData["ErrorMessage"] = "FAQ not found.";
					return RedirectToAction(nameof(Index));
				}

				// Xóa ảnh cũ nếu có
				if (!string.IsNullOrEmpty(faq.Image))
				{
					string oldImagePath = Path.Combine(_env.WebRootPath, "images/FAQ", faq.Image);
					if (IOFile.Exists(oldImagePath))
					{
						IOFile.Delete(oldImagePath);
					}
				}

				_context.FAQs.Remove(faq);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "FAQ deleted successfully.";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				TempData["ErrorMessage"] = "An error occurred while deleting the FAQ.";
			}

			return RedirectToAction(nameof(Index));
		}



	}
}
