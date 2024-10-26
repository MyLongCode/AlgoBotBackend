using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AlgoBotBackend.Controllers
{
	[Authorize]
    public class FirmController : Controller
	{
		private readonly DBContext _db;
		private readonly ILogger<FirmController> _logger;

		public FirmController(ILogger<FirmController> logger, DBContext db)
		{
			_logger = logger;
			_db = db;
		}
        public async Task<ActionResult> Index()
		{
			return View(await _db.Firms.ToListAsync());
		}

        [HttpGet("/firm/{id}/details")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return NotFound();
            var firm = await _db.Firms.Include(f => f.Owner).FirstOrDefaultAsync(u => u.Id == id);
            if (firm == null) return NotFound();
            return View(firm);
        }

        [HttpGet("/firm")]
        public async Task<ActionResult> GetAllFirms()
        {
            return Ok(await _db.Firms.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

		[HttpPost("/firm/create")]
		public async Task<IActionResult> CreateFirm(CreateFirmModel dto)
		{
			var owner = _db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
			var firm = new Firm()
			{
				OwnerId = owner.Id,
				Owner = owner,
				Name = dto.Name,
			};
			await _db.Firms.AddAsync(firm);
			await _db.SaveChangesAsync();
			return RedirectToAction("Index");
		}
	}
}
