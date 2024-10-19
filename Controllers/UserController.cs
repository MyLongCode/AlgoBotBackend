using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Web.Http.Routing;

namespace AlgoBotBackend.Controllers
{
    public class UserController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Users.ToListAsync());
        }

        [HttpGet("/User/Details/{username}")]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null) return NotFound();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet("/user")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _db.Users.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
