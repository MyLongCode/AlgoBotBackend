using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlgoBotBackend.Controllers
{
    [Authorize(Roles = "botuser")]
    public class ProfileController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }
        [HttpGet("/profile")]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            if (username == null) return NotFound();
            var allUsers = await _db.Users.ToListAsync();
            var user = allUsers.FirstOrDefault(u => u.Login == username);
            var userId = user.Id;
            if (user == null) return NotFound();
            var allPayments = await _db.Payments.ToListAsync();
            user.Payments = allPayments.Where(x => x.UserId == userId).ToList();

            var referals1 = allUsers
                .Where(x => x.ReferalUsername == username)
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
                Сashback = user.Cashback,
                Score = user.Score,
                Referals1 = referals1,
                Referals2 = referals2,
                Referals3 = referals3,
            };

            return View(viewmodel);
        }

    }
}
