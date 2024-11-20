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
            var botUsers = await _db.Users.Where(b => b.Role == "botuser").ToListAsync();
            return View(botUsers);
        }

        [HttpGet("/user/{username}/details")]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null) return NotFound();
            var allUsers = await _db.Users.ToListAsync();
            var user = allUsers.FirstOrDefault(u => u.Login == username);
            var userId = user.Id;
            if (user == null) return NotFound();
            var allPayments = await _db.Payments.ToListAsync();
            user.Payments = allPayments.Where(x => x.UserId == userId).ToList();

            var referals1 = allUsers
                .Where(x => x.ReferalUsername ==  username)
                .Select(x =>
                {
                    x.Payments = allPayments.Where(y => y.UserId == x.Id).ToList();
                    return x;
                })
                .ToList();
            var referals1Names = referals1.Select(x => x.Login).ToList();
            var referals2 = allUsers
                .Where(x => referals1Names.Contains(x.ReferalUsername))
                .Select(x =>
                {
                    x.Payments = allPayments.Where(y => y.UserId == x.Id).ToList();
                    return x;
                })
                .ToList();
            var referals2Names = referals2.Select(x => x.Login).ToList();
            var referals3 = allUsers.Where(x => referals2Names.Contains(x.ReferalUsername))
                .Select(x => { x.Payments = allPayments.Where(y => y.UserId == x.Id).ToList(); return x; }).ToList();


            var viewmodel = new BotUserViewModel
            {
                Username = user.Login,
                ReferalUsername = user.ReferalUsername,
                Firstname = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ChildAge = user.ChildAge,
                ChildName = user.ChildName,
                Ñashback = user.Cashback,
                Score = user.Score,
                Referals1 = referals1,
                Referals2 = referals2,
                Referals3 = referals3,
            };

            return View(viewmodel);
        }

        [HttpGet("/user/{username}/edit")]
        public async Task<IActionResult> Edit(string username)
        {
            if (username == null) return NotFound();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == username);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost("/user/{username}/edit")]
        public async Task<IActionResult> Edit(string username, int score)
        {
            if (username == null) return NotFound();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == username);
            if (user == null) return NotFound();
            user.Score += score;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return View("Edit", user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
