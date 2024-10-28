using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Migrations.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AlgoBotBackend.Models.ViewModels;

namespace AlgoBotBackend.Controllers
{
    public class AuthController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<BotUserController> _logger;

        public AuthController(ILogger<BotUserController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Denied()
        {
            return View();
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetLogin(AuthViewModel dto)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Login== dto.Login && u.Password == dto.Password);
                if (user != null)
                {
                    await Authenticate(dto.Login, user.Role); // аутентификация

                    return RedirectToAction("Index", "Campaign");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(dto);
        }

        private async Task Authenticate(string userName, string role)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
