using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlgoBotBackend.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }
        
        //public async Task<IActionResult> Index()
        //{
        //    var username = User.Identity.Name;
        //    if (username == null) return NotFound();
        //    var user = await _db.BotUsers.Include(b => b.Сampaign).FirstOrDefaultAsync(u => u.Username == username);
        //    if (user == null) return NotFound();
        //    var countReferals = 0;
        //    double cashback = 0;
        //    var referals = _db.BotUsers.Where(u => u.ReferalUsername == user.Username).ToList();
        //    var referals2 = _db.BotUsers.Where(u => referals.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
        //    var referals3 = _db.BotUsers.Where(u => referals2.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
        //    if (user.Сampaign.ReferalSystem == ReferalSystem.OneLevel)
        //    {
        //        countReferals = referals.Count();
        //        cashback += GetCashback(cashback, 100, referals, user.Сampaign);
        //    }
        //    if (user.Сampaign.ReferalSystem == ReferalSystem.TwoLevel)
        //    {
        //        var procents = user.Сampaign.Distribution.Split("/").Select(p => double.Parse(p)).ToList();
        //        var procent = procents[0];
        //        countReferals = referals.Count();

        //        cashback += GetCashback(cashback, procent, referals, user.Сampaign);
        //        procent = procents[1];

        //        countReferals += referals2.Count();
        //        cashback += GetCashback(cashback, procent, referals2, user.Сampaign);
        //    }

        //    if (user.Сampaign.ReferalSystem == ReferalSystem.ThreeLevel)
        //    {
        //        var procents = user.Сampaign.Distribution.Split("/").Select(p => double.Parse(p)).ToList();
        //        var procent = procents[0];
        //        countReferals += referals.Count();
        //        countReferals += referals2.Count();
        //        countReferals += referals3.Count();

        //        cashback += GetCashback(cashback, procent, referals, user.Сampaign);
        //        procent = procents[1];

        //        cashback += GetCashback(cashback, procent, referals2, user.Сampaign);
        //        procent = procents[2];

        //        cashback += GetCashback(cashback, procent, referals3, user.Сampaign);
        //    }

        //    var viewmodel = new BotUserViewModel()
        //    {
        //        Username = user.Username,
        //        ReferalUsername = user.ReferalUsername,
        //        Firstname = user.Firstname,
        //        PhoneNumber = user.PhoneNumber,
        //        ChildAge = user.ChildAge,
        //        ChildName = user.ChildName,
        //        Score = user.Score,
        //        CampaignId = user.CampaignId,
        //        Сampaign = user.Сampaign,
        //        CountReferals = countReferals,
        //        Сashback = (int)cashback,
        //        Referals1 = referals,
        //        Referals2 = referals2,
        //        Referals3 = referals3,
        //    };
        //    return View(viewmodel);
        //}
        public double GetCashback(double cashback, double procent, IEnumerable<BotUser> referals, AdvertisingСampaign campaign)
        {
            var countReferals = referals.Count();
            if (campaign.ProcentScore != null) cashback += (int)(referals.Sum(r => r.Score) * campaign.ProcentScore / 100) * procent / 100;
            else cashback = (int)(countReferals * campaign.Score) * procent / 100;

            return cashback;
        }
    }
}
