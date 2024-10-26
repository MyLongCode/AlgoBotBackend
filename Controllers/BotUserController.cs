using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
namespace AlgoBotBackend.Controllers
{
    [Authorize]
    public class BotUserController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<BotUserController> _logger;

        public BotUserController(ILogger<BotUserController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }
        
        [HttpGet("/user/index")]
        public async Task<IActionResult> Index()
        {
            return View(await _db.BotUsers.ToListAsync());
        }

        [HttpGet("/user/{username}/details")]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null) return NotFound();
            var user = await _db.BotUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet("/user")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _db.BotUsers.ToListAsync());
        }

        [HttpGet("/user/{username}/edit")]
        public async Task<IActionResult> Edit(string username)
        {
            if (username == null) return NotFound();
            var user = await _db.BotUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost("/user/{username}/edit")]
        public async Task<IActionResult> Edit(string username, int score)
        {
            if (username == null) return NotFound();
            var user = await _db.BotUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            user.Score += score;
            _db.BotUsers.Update(user);
            await _db.SaveChangesAsync();
            return View("Edit", user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
