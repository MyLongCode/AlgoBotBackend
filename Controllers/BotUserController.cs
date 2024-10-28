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
            return View(await _db.BotUsers.Include(b => b.Сampaign).ToListAsync());
        }

        [HttpGet("/user/{username}/details")]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null) return NotFound();
            var user = await _db.BotUsers.Include(b => b.Сampaign).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            var countReferals = 0;
            var cashback = 0;
            if (user.Сampaign.ReferalSystem == ReferalSystem.OneLevel) 
            {
                var referals = _db.BotUsers.Where(u => u.ReferalUsername == user.Username).ToList();
                countReferals = referals.Count();
                if (user.Сampaign.ProcentScore != null) cashback = (int)(referals.Sum(r => r.Score) * user.Сampaign.ProcentScore / 100);
                else cashback = (int)(referals.Count * user.Сampaign.Score);
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
                Сampaign = user.Сampaign,
                CountReferals = countReferals,
                Сashback = cashback,
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
        public async Task<IActionResult> RandomCreate()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var names = new string[] { "Данил", "Дима", "Сергей", "Вова", "Вадим", "Влад", "Роман", "Кирилл", "Игорь", "Ваня" };
            var surnames = new string[] { "Олегович", "Данилович", "Дмитриевич", "Алексеевич", "Антонович", "Сергеевич", "Константинович", "Владимирович", "Павлович" };
            for (int i = 0; i < 10; i++)
            {
                var username = new string(Enumerable.Repeat(chars, random.Next(4, 14))
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                var referalUsername = _db.BotUsers.ToList()[random.Next(0,10)].Username;
                var name = $"{names[random.Next(names.Length)]} {surnames[random.Next(surnames.Length)]}";
                var phone = random.NextInt64(80000000000, 89999999999).ToString();
                var childName = names[random.Next(names.Length)];
                var childAge = random.Next(10, 17).ToString();
                var score = random.Next(10000, 40000);
                var campaignId = random.Next(1, _db.AdvertisingСampaigns.Count() + 1);
                var campaign = _db.AdvertisingСampaigns.Find(campaignId);
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
                    Сampaign = campaign,
                    StageReg = 5
                };
                _db.BotUsers.Add(user);
                _db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
