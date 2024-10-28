using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models;
using AlgoBotBackend.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
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
            return View(await _db.BotUsers.Include(b => b.Ñampaign).ToListAsync());
        }

        [HttpGet("/user/{username}/details")]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null) return NotFound();
            var user = await _db.BotUsers.Include(b => b.Ñampaign).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            var countReferals = 0;
            double cashback = 0;
            if (user.Ñampaign.ReferalSystem == ReferalSystem.OneLevel) 
            {
                var referals = _db.BotUsers.Where(u => u.ReferalUsername == user.Username).ToList();
                countReferals = referals.Count();
                cashback += GetCashback(cashback, 100, referals, user.Ñampaign);
            }
            if (user.Ñampaign.ReferalSystem == ReferalSystem.TwoLevel)
            {
                var procents = user.Ñampaign.Distribution.Split("/").Select(p => double.Parse(p)).ToList();
                var procent = procents[0];
                var referals = _db.BotUsers.Where(u => u.ReferalUsername == user.Username).ToList();
                var referals2 = _db.BotUsers.Where(u => referals.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
                countReferals = referals.Count();

                cashback += GetCashback(cashback, procent, referals, user.Ñampaign);
                procent = procents[1];
                
                countReferals += referals2.Count();
                cashback += GetCashback(cashback, procent, referals2, user.Ñampaign);
            }

            if (user.Ñampaign.ReferalSystem == ReferalSystem.ThreeLevel)
            {
                var procents = user.Ñampaign.Distribution.Split("/").Select(p => double.Parse(p)).ToList();
                var procent = procents[0];
                var referals = _db.BotUsers.Where(u => u.ReferalUsername == user.Username).ToList();
                var referals2 = _db.BotUsers.Where(u => referals.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
                var referals3 = _db.BotUsers.Where(u => referals2.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
                countReferals += referals.Count();
                countReferals += referals2.Count();
                countReferals += referals3.Count();

                cashback += GetCashback(cashback, procent, referals, user.Ñampaign);
                procent = procents[1];
                
                cashback += GetCashback(cashback, procent, referals2, user.Ñampaign);
                procent = procents[2];
                
                cashback += GetCashback(cashback, procent, referals3, user.Ñampaign);
            }

            var viewmodel = new BotUserViewModel()
            {
                Username = user.Username,
                ReferalUsername = user.ReferalUsername,
                Firstname = user.Firstname,
                PhoneNumber = user.PhoneNumber,
                ChildAge = user.ChildAge,
                ChildName = user.ChildName,
                Score = user.Score,
                CampaignId = user.CampaignId,
                Ñampaign = user.Ñampaign,
                CountReferals = countReferals,
                Ñashback = (int)cashback,
            };
            return View(viewmodel);
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

        [HttpPost("/user/randomcreate")]
        public IActionResult RandomCreate()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var names = new string[] { "Äàíèë", "Äèìà", "Ñåğãåé", "Âîâà", "Âàäèì", "Âëàä", "Ğîìàí", "Êèğèëë", "Èãîğü", "Âàíÿ" };
            var surnames = new string[] { "Îëåãîâè÷", "Äàíèëîâè÷", "Äìèòğèåâè÷", "Àëåêñååâè÷", "Àíòîíîâè÷", "Ñåğãååâè÷", "Êîíñòàíòèíîâè÷", "Âëàäèìèğîâè÷", "Ïàâëîâè÷" };
            for (int i = 0; i < 10; i++)
            {
                var username = new string(Enumerable.Repeat(chars, random.Next(4, 14))
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                var referalUsername = _db.BotUsers.ToList()[random.Next(0, 10)].Username;
                //var referalUsername = new string(Enumerable.Repeat(chars, random.Next(4, 14))
                //    .Select(s => s[random.Next(s.Length)]).ToArray());
                var name = $"{names[random.Next(names.Length)]} {surnames[random.Next(surnames.Length)]}";
                var phone = random.NextInt64(80000000000, 89999999999).ToString();
                var childName = names[random.Next(names.Length)];
                var childAge = random.Next(10, 17).ToString();
                var score = random.Next(10000, 40000);
                var campaignId = random.Next(10, 10 + _db.AdvertisingÑampaigns.Count());
                var campaign = _db.AdvertisingÑampaigns.Find(campaignId);
                var user = new BotUser()
                {
                    Username = username,
                    ReferalUsername = referalUsername,
                    Firstname = name,
                    PhoneNumber = phone,
                    ChildAge = childAge,
                    ChildName = childName,
                    Score = score,
                    CampaignId = campaignId,
                    Ñampaign = campaign,
                    StageReg = 5
                };
                _db.BotUsers.Add(user);
                _db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        public double GetCashback(double cashback, double procent, IEnumerable<BotUser> referals, AdvertisingÑampaign campaign)
        {
            var countReferals = referals.Count();
            if (campaign.ProcentScore != null) cashback +=  (int)(referals.Sum(r => r.Score) * campaign.ProcentScore / 100) * procent / 100;
            else cashback = (int)(countReferals * campaign.Score) * procent / 100;

            return cashback;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
