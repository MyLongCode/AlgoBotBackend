using AlgoBotBackend.Migrations.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgoBotBackend.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class PaymentController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> AddPayments()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayments(IFormFile file)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
