using EP1.Models;
using EP1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EP1.Areas.System.Controllers
{
    [Area("System")]
    [Route("/System/Account")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<Account> _userManager;
        public AccountController(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }
        // Action để lấy danh sách tài khoản

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách tài khoản từ UserManager
            var users = await _userManager.Users.ToListAsync(); ;

            // Chuyển đổi sang danh sách AccountViewModel
            var userViewModels = users.Select(user => new AccountViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = Enum.IsDefined(typeof(ViewModels.UserRole), user.Role)
                        ? (ViewModels.UserRole)user.Role
                        : ViewModels.UserRole.Student
            }).ToList();

            // Trả về View với model là AccountViewModel
            return View(userViewModels);
        }
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new AccountViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = Enum.IsDefined(typeof(ViewModels.UserRole), user.Role)
                        ? (ViewModels.UserRole)user.Role
                        : ViewModels.UserRole.Student,
                Class = user.Class,
                Specification = user.Specification,
                Section = user.Section,
            };

            return View(viewModel);
        }
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(AccountViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == viewModel.Email);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = viewModel.FullName;
            user.IsActive = viewModel.IsActive;
            user.Role = (int)viewModel.Role; 
            user.Class = viewModel.Class;
            user.Specification = viewModel.Specification;
            user.Section = viewModel.Section;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(viewModel);
        }
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
          
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == id);
            if (user == null)
            {
                return NotFound();
            }

          
            var viewModel = new AccountViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = Enum.IsDefined(typeof(ViewModels.UserRole), user.Role)
                        ? (ViewModels.UserRole)user.Role
                        : ViewModels.UserRole.Student
            };

            return View(viewModel); 
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == id);
            if (user == null)
            {
                return NotFound();
            }

            
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

          
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            
            return View(await Delete(id));
        }

    }
}
