using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models;
using AlgoBotBackend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlgoBotBackend.Controllers
{
    [Authorize]
    public class CampaignController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<CampaignController> _logger;

        public CampaignController(ILogger<CampaignController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var campaigns = await _db.AdvertisingСampaigns.Include(c => c.Firm).Include(c => c.Courses).OrderBy(c => c.Firm.Name).ToListAsync();
            var authUser = _db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            if (!User.IsInRole("admin")) campaigns = campaigns.Where(c => c.Firm.OwnerId == authUser.Id).ToList();
            return View(campaigns);
        }

        [HttpGet("/firm/{firmId}/compaign")]
        public async Task<IActionResult> Create([FromRoute] int firmId)
        {
            var compaign = new CreateCampaignViewModel { FirmId = firmId };
            var compaigns = _db.AdvertisingСampaigns.Include(c => c.Courses).Where(c => c.FirmId == firmId).ToList();
            var allCourses = _db.Courses.Include(c => c.Сampaigns).ToList();
            ViewBag.AllCourses = allCourses.Where(course => !compaigns.Any(comp => comp.Courses.Contains(course))).ToList();
            return View(compaign);
        }

        [HttpPost("/campaign/create")]
        public async Task<IActionResult> CreateCampaign(CreateCampaignViewModel dto)
        {
            var firm = _db.Firms.FirstOrDefault(u => u.Id == dto.FirmId);
            var courses = _db.Courses.Where(c => dto.Courses.Contains(c.Name)).ToList();
            if (dto.ReferalSystem == ReferalSystem.OneLevel) dto.Distribution = "100";
            var campaign = new AdvertisingСampaign()
            {
                FirmId = dto.FirmId,
                Firm = firm,
                Name = dto.Name,
                ReferalSystem = dto.ReferalSystem,
                Distribution = dto.Distribution,
                Courses = courses
            };
            if (dto.ScoreType == ScoreType.Summa) campaign.Score = dto.Summ;
            else campaign.ProcentScore = dto.Summ;
            await _db.AdvertisingСampaigns.AddAsync(campaign);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("/campaign/{id}/details")]
        public async Task<IActionResult> Details(int id)
        {
            var campaign = _db.AdvertisingСampaigns.Include(x => x.Firm).FirstOrDefault(x => x.Id == id);
            var users = _db.Users.ToList();
            var payments = _db.Payments
                .Where(x => x.CampaignId == campaign.Id)
                .ToList();
            
            var viewmodel = new CampaignViewModel
            {
                Name = campaign.Name,
                Firm = campaign.Firm,
                ReferalSystem = campaign.ReferalSystem,
                ProcentScore = campaign.ProcentScore,
                Score = campaign.Score,
                CountUsers = payments.GroupBy(x => x.UserId).Select(x => x.First()).ToList().Count,
                ScoreSumm = payments.Sum(x => x.Amount),
                Payments = payments.Select(x => new PaymentViewModel
                {
                    Id = x.Id,
                    Username = users.First(y => y.Id == x.UserId).Login,
                    Fullname = users.First(y => y.Id == x.UserId).FullName,
                    CampaignName = "",
                    Amount = x.Amount,
                }).ToList()
            };
            return View(viewmodel);
        }

        [HttpPost("/campaign/{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var campaign = _db.AdvertisingСampaigns.Find(id);
            var firm = _db.Firms.FirstOrDefault(f => f.Id == campaign.FirmId);
            var owner = _db.Users.FirstOrDefault(x => x.Id == firm.OwnerId);
            if (User.Identity.Name == owner.Login)
            {
                _db.Firms.Remove(firm);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
