using EP1.Models;
using EP1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IOFile = System.IO.File;

namespace EP1.Areas.System.Controllers
{
    [Area("System")]
    [Route("/System/Blog")]
	[Authorize(Roles = "Admin")]
	public class BlogController : Controller
    {
        private readonly ShopContext _context;
        private readonly IWebHostEnvironment _env;
        public BlogController(ShopContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var blogs = _context.Blogs.ToList();

            return View(blogs);
        }
        [HttpGet("Add")]
        public IActionResult Add()
        {
            var blogviewmodel = new BlogViewModel();
            return View(blogviewmodel);
            
        }

        // Xử lý thêm bài viết
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BlogViewModel model, IFormFile imgFile)
        {
            ModelState.Remove("Image");
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (imgFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
                    string extension = Path.GetExtension(imgFile.FileName);
                    string uniqueFileName = fileName + "_" + Guid.NewGuid().ToString() + extension;

                    string filePath = Path.Combine(_env.WebRootPath, "images/Blog", uniqueFileName);
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images/Blog"));

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

                // Thêm Blog vào cơ sở dữ liệu
                var blog = new Blog
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    Image = model.Image
                };

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Blog created successfully.";
                return RedirectToAction("Index");

            }
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
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            var model = new BlogViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Image = blog.Image // Đường dẫn ảnh hiện tại
            };

            return View(model);
        }

        [HttpPost]
        [Route("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogViewModel model, IFormFile? imgFile)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Invalid Blog ID.";
                return BadRequest();
            }

            // Loại bỏ lỗi cho Image nếu không upload ảnh mới
            if (imgFile == null)
            {
                ModelState.Remove("Image");
            }

            if (ModelState.IsValid)
            {
                var blog = await _context.Blogs.FindAsync(id);

                if (blog == null)
                {
                    TempData["ErrorMessage"] = "Blog not found.";
                    return NotFound();
                }

                // Xử lý upload ảnh mới
                if (imgFile != null)
                {
                    try
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(blog.Image))
                        {
                            string oldImagePath = Path.Combine(_env.WebRootPath, "images/Blog", blog.Image);
                            if (IOFile.Exists(oldImagePath))
                            {
                                IOFile.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName);
                        string extension = Path.GetExtension(imgFile.FileName);
                        string newImagePath = fileName + "_" + Guid.NewGuid().ToString() + extension;
                        string newFilePath = Path.Combine(_env.WebRootPath, "images/Blog", newImagePath);

                        Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images/Blog"));

                        using (var stream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await imgFile.CopyToAsync(stream);
                        }

                        blog.Image = newImagePath;
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
                    blog.Image = model.Image;
                }

                // Cập nhật các trường khác
                blog.Title = model.Title;
                blog.Description = model.Description;

                try
                {
                    _context.Blogs.Update(blog);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Blog updated successfully.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database Save Error: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while updating the Blog.";
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
                var blog = await _context.Blogs.FindAsync(id);

                if (blog == null)
                {
                    TempData["ErrorMessage"] = "Blog not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(blog.Image))
                {
                    string oldImagePath = Path.Combine(_env.WebRootPath, "images/Blog", blog.Image);
                    if (IOFile.Exists(oldImagePath))
                    {
                        IOFile.Delete(oldImagePath);
                    }
                }

                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Blog deleted successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the Blog.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
