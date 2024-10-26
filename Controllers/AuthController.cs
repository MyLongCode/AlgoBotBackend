using AlgoBotBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Migrations.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthViewModel dto
            )
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Login== dto.Login && u.Password == dto.Password);
                if (user != null)
                {
                    await Authenticate(dto.Login); // аутентификация

                    return RedirectToAction("index", "user");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(dto);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

    }
}
