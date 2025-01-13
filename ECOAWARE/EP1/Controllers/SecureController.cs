using EP1.Models;
using EP1.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EP1.Controllers
{
    public class SecureController : Controller
    {
        private readonly ShopContext db;
        private readonly UserManager<Account> _userManager; //quản lý user
        private readonly SignInManager<Account> _signInManager; //quản qt đăng nhập của user
        private readonly RoleManager<IdentityRole> _roleManager; //quản lý Role(Vai trò user)
        public SecureController(ShopContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpGet("/login", Name = "dangnhap")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email ?? "");
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email.");
                    return View(model);
                }
                else
                {
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError("", "Your account is not activated. Please contact support.");
                        return View(model);
                    }
                    var remember = model.RememberMe;
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName ?? "",
                        model.Password ?? "",
                        isPersistent: remember,
                        lockoutOnFailure: true
                    );
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Web");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Password.");
                        return View(model);
                    }
                }

            }
            return View(model);
        }

        [Route("/register", Name = "dangky")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email ?? "");
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already in use");
                    return View(model);
                }

                var account = new Account
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Class = model.Class,
                    Specification = model.Specification,
                    Section = model.Section,
                    Role = (int)model.Role,
                    IsActive = false,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(account, model.Password ?? "");
                if (result.Succeeded)
                {
                    // Kiểm tra nếu Role chưa tồn tại, tạo mới
                    if (!await _roleManager.RoleExistsAsync(model.Role.ToString()))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role.ToString()));
                    }

                    // Gán Role cho người dùng
                    await _userManager.AddToRoleAsync(account, model.Role.ToString());

                    // Đăng nhập người dùng
                    //await _signInManager.SignInAsync(account, isPersistent: false);


                    return RedirectToAction("Index", "Web");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet("/logout", Name = "dangxuat")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Web");
        }

        [HttpGet("/profile", Name = "profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);  // Lấy thông tin người dùng từ Cookie hoặc Session
            if (user == null)
            {
                return RedirectToAction("Login", "Secure");
            }

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Class = user.Class,
                Specification = user.Specification,
                Section = user.Section
            };
            ViewData["UserName"] = user.FullName;

            return View(model);
        }

        // Action để xử lý cập nhật thông tin
        [HttpPost("/profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Secure");
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Class = model.Class;
                user.Specification = model.Specification;
                user.Section = model.Section;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Update Success!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View("Profile", model);
        }

		[HttpGet("/access-denied")]
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
