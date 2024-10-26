using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlgoBotBackend.Controllers
{
    public class CampaignController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<CampaignController> _logger;

        public CampaignController(ILogger<CampaignController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<ActionResult> Index()
        {
            return View(await _db.AdvertisingСampaigns.Include(c => c.Firm).ToListAsync());
        }

        [HttpGet("/campaign/{id}/details")]
        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return NotFound();
            var campaign = await _db.AdvertisingСampaigns.Include(f => f.Firm).FirstOrDefaultAsync(u => u.Id == id);
            if (campaign == null) return NotFound();
            return View(campaign);
        }
        [HttpGet("/firm/{firmId}/compaign")]
        public async Task<IActionResult> Create([FromRoute] int firmId)
        {
            var compaign = new CreateCampaignViewModel { FirmId = firmId };
            return View(compaign);
        }

        [HttpPost("/campaign/create")]
        public async Task<IActionResult> CreateCampaign(CreateCampaignViewModel dto)
        {
            var firm = _db.Firms.FirstOrDefault(u => u.Id == dto.FirmId);
            var campaign = new AdvertisingСampaign()
            {
                FirmId = dto.FirmId,
                Firm = firm,
                Name = dto.Name,
                ReferalSystem = dto.ReferalSystem,
            };
            if (dto.ScoreType == ScoreType.Summa) campaign.Score = dto.Summ;
            else campaign.ProcentScore = dto.Summ;
            await _db.AdvertisingСampaigns.AddAsync(campaign);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
