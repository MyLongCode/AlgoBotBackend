using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.ViewModels;
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

        public async Task<IActionResult> Index()
		{
            var firms = _db.Firms.Include(f => f.Owner).OrderBy(x => x.Name).ToList();
            if (!User.IsInRole("admin")) firms = firms.Where(f => f.Owner.Login == User.Identity.Name).ToList();
			return View(firms);
		}

        [HttpGet("/firm/{id}/details")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return NotFound();
            var firm = await _db.Firms.Include(f => f.Owner).FirstOrDefaultAsync(u => u.Id == id);
            if (firm == null) return NotFound();
            return View(firm);
        }

        [HttpGet("/firm/{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null) return NotFound();
            var firm = await _db.Firms.FirstOrDefaultAsync(u => u.Id == id);
            if (firm == null) return NotFound();
            var viewmodel = new EditFirmViewModel() { Name = firm.Name, FirmId = id };
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> EditFirm(EditFirmViewModel dto)
        {
            var firm = await _db.Firms.Include(f => f.Owner).FirstOrDefaultAsync(f => f.Id == dto.FirmId);
            firm.Name = dto.Name;
			_db.Firms.Update(firm);
			_db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

		[HttpPost("/firm/create")]
		public async Task<IActionResult> CreateFirm(CreateFirmViewModel dto)
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
