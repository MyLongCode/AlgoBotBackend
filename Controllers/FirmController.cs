using AlgoBotBackend.Migrations.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Create()
        {
            return View();
        }

		//[HttpPost("/firm/create")]
		//public async Task<IActionResult> Create()
		//{

		//}
    }
}
