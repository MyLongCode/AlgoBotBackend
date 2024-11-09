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
            var botUsers = await _db.BotUsers.ToListAsync();
            return View(botUsers);
        }

        //[HttpGet("/user/{username}/details")]
        //public async Task<IActionResult> Details(string username)
        //{
        //    if (username == null) return NotFound();
        //    var user = await _db.BotUsers.Include(b => b.Ñampaign).FirstOrDefaultAsync(u => u.Username == username);
        //    if (user == null) return NotFound();
        //    var countReferals = 0;
        //    double cashback = 0;
        //    var referals = _db.BotUsers.Where(u => u.ReferalUsername == user.Username).ToList();
        //    var referals2 = _db.BotUsers.Where(u => referals.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
        //    var referals3 = _db.BotUsers.Where(u => referals2.Select(r => r.Username).ToList().Contains(u.ReferalUsername)).ToList();
        //    if (user.Ñampaign.ReferalSystem == ReferalSystem.OneLevel)
        //    {
        //        countReferals = referals.Count();
        //        cashback += GetCashback(cashback, 100, referals, user.Ñampaign);
        //    }
        //    if (user.Ñampaign.ReferalSystem == ReferalSystem.TwoLevel)
        //    {
        //        var procents = user.Ñampaign.Distribution.Split("/").Select(p => double.Parse(p)).ToList();
        //        var procent = procents[0];
        //        countReferals = referals.Count();

        //        cashback += GetCashback(cashback, procent, referals, user.Ñampaign);
        //        procent = procents[1];

        //        countReferals += referals2.Count();
        //        cashback += GetCashback(cashback, procent, referals2, user.Ñampaign);
        //    }

        //    if (user.Ñampaign.ReferalSystem == ReferalSystem.ThreeLevel)
        //    {
        //        var procents = user.Ñampaign.Distribution.Split("/").Select(p => double.Parse(p)).ToList();
        //        var procent = procents[0];
        //        countReferals += referals.Count();
        //        countReferals += referals2.Count();
        //        countReferals += referals3.Count();

        //        cashback += GetCashback(cashback, procent, referals, user.Ñampaign);
        //        procent = procents[1];

        //        cashback += GetCashback(cashback, procent, referals2, user.Ñampaign);
        //        procent = procents[2];

        //        cashback += GetCashback(cashback, procent, referals3, user.Ñampaign);
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
        //        Ñampaign = user.Ñampaign,
        //        CountReferals = countReferals,
        //        Ñashback = (int)cashback,
        //        Referals1 = referals,
        //        Referals2 = referals2,
        //        Referals3 = referals3,
        //    };
        //    return View(viewmodel);
        //}

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
