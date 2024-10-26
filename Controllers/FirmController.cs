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
		private readonly ILogger<BotUserController> _logger;

		public FirmController(ILogger<BotUserController> logger, DBContext db)
		{
			_logger = logger;
			_db = db;
		}

        public async Task<ActionResult> Index()
		{
			return View(await _db.Firms.ToListAsync());
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
		public async Task<IActionResult> Create(CreateFirmModel dto)
		{
			var firm = new Firm()
			{
				Owner = _db.Users.FirstOrDefault(u => u.Login == ),
				Name = dto.Name,
				DefaultReferalSystem = dto.DefaultReferalSystem
			};
		}
	}
}
