using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using forum.Models;

namespace forum.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appEnvironment = appEnvironment;
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string userName, string email, string password, DateTime birthDate, IFormFile? avatar)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Заполните все обязательные поля.");
                return View();
            }

            string avatarPath = "/images/default-avatar.png";

            if (avatar != null && avatar.Length > 0)
            {
                string uploadsFolder = Path.Combine(_appEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatar.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                avatarPath = "/images/" + uniqueFileName;
            }

            var user = new User
            {
                UserName = userName,
                Email = email,
                BirthDate = DateTime.SpecifyKind(birthDate, DateTimeKind.Utc),
                Avatar = avatarPath
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Topics");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
        
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string emailOrUserName, string password, bool rememberMe, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(emailOrUserName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Введите логин/email и пароль.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(emailOrUserName) ?? await _userManager.FindByNameAsync(emailOrUserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Topics");
                }
            }

            ModelState.AddModelError("", "Неверный логин/email или пароль.");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Topics");
        }
    }
}