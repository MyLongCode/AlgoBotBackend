using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.CSV;
using AlgoBotBackend.Models.ViewModels;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using NuGet.Versioning;
using System.Globalization;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> Index()
        {
            var ownerId = _db.Users.FirstOrDefault(x => x.Login == User.Identity.Name).Id;
            var firm = _db.Firms.FirstOrDefault(x => x.OwnerId == ownerId);
            var campaigns = _db.AdvertisingСampaigns.Where(x => x.FirmId == firm.Id).ToList();
            var campaignsId = campaigns.Select(x => x.Id).ToList();
            var users = _db.Users.ToList();
            var payments = _db.Payments.Where(x => campaignsId.Contains(x.CampaignId)).ToList();
            var viewmodel = payments.Select(x => new PaymentViewModel
            {
                Id = x.Id,
                Username = users.First(y => y.Id == x.UserId).Login,
                Fullname = users.First(y => y.Id == x.UserId).FullName,
                CampaignName = campaigns.First(y => y.Id == x.CampaignId).Name,
                Amount = x.Amount,
            });

            return View(viewmodel);
        }

        [HttpGet]
        public async Task<IActionResult> AddPayments()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayments(AddPaymentViewModel dto)
        {
            if (dto.Payments == null) ModelState.AddModelError("", "Файл оплат не загружен.");
            if (dto.Students == null) ModelState.AddModelError("", "Файл учеников не загружен.");
            if(!ModelState.IsValid) return View("AddPayments");

            try
            {
                var readerPayments = new StreamReader(dto.Payments.OpenReadStream());
                var readerStudents = new StreamReader(dto.Students.OpenReadStream());
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = true };
                var csvPayments = new CsvReader(readerPayments, config);
                var csvStudents = new CsvReader(readerStudents, config);
                var payments = csvPayments.GetRecords<PaymentCSV>().ToList();
                var students = csvStudents.GetRecords<StudentCSV>().ToList();

                var users = await _db.Users.ToListAsync();
                var ownerId = users.FirstOrDefault(x => x.Login == User.Identity.Name).Id;
                var campaigns = await _db.AdvertisingСampaigns.Include(x => x.Firm).Include(x => x.Courses).Where(x => x.Firm.OwnerId == ownerId).ToListAsync();
                var courses = await _db.Courses.ToListAsync();

                students = students
                    .Where(s => s.Phonenumber.Length > 10)
                    .Where(s => payments.FirstOrDefault(p => p.StudentId == s.StudentId) != null)
                    .Select(s =>
                {
                    s.Phonenumber = s.Phonenumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Replace(" ", "").Replace("+", "");
                    return s;
                })
                    .Where(s => users.FirstOrDefault(p => p.PhoneNumber == s.Phonenumber) != null)
                    .ToList();



                var studentPayments = payments
                    .Where(s => students.FirstOrDefault(p => p.StudentId == s.StudentId) != null)
                    .Where(s => campaigns.Where(c => c.Courses.Select(x => x.Name).Contains(s.CourseName)) != null)
                    .Select(s => new StudentPaymentsCSV
                    {
                        Amount = (int)s.Amount,
                        Campaign = campaigns.FirstOrDefault(c => c.Courses.Select(x => x.Name).Contains(s.CourseName)),
                        PhoneNumber = students.FirstOrDefault(x => x.StudentId == s.StudentId).Phonenumber
                    }).ToList();



                var finishPayments = studentPayments
                    .Where(s => s.Campaign != null)
                    .Select(s => new Payment()
                    {
                        UserId = users.FirstOrDefault(u => u.PhoneNumber == s.PhoneNumber).Id,
                        Amount = s.Amount,
                        CampaignId = s.Campaign.Id,
                    }).ToList();

                foreach (var user in users)
                    user.Score += finishPayments.Where(p => p.UserId == user.Id).Sum(p => p.Amount);

                foreach (var payment in finishPayments)
                {
                    var campaign = campaigns.FirstOrDefault(x => x.Id == payment.CampaignId);
                    var user = users.FirstOrDefault(x => x.Id == payment.UserId);
                    var procents = campaign.Distribution.Split('/').Select(x => int.Parse(x)).ToList();

                    var referal = users.FirstOrDefault(x => x.Login == user.ReferalUsername);
                    if (referal == null) continue;
                    if (campaign.ProcentScore != null) referal.Cashback += (int)(payment.Amount * campaign.ProcentScore * procents[0] / 10000);
                    if (campaign.Score != null) referal.Cashback += (int)(campaign.Score * procents[0] / 100);

                    if (campaign.ReferalSystem != ReferalSystem.OneLevel)
                    {
                        var referal2 = users.FirstOrDefault(user => user.Login == referal.ReferalUsername);
                        if (referal2 == null) continue;
                        if (campaign.ProcentScore != null) referal2.Cashback += (int)(payment.Amount * campaign.ProcentScore * procents[1] / 10000);
                        if (campaign.Score != null) referal2.Cashback += (int)(campaign.Score * procents[1] / 100);

                        if (campaign.ReferalSystem == ReferalSystem.ThreeLevel)
                        {
                            var referal3 = users.FirstOrDefault(user => user.Login == referal2.ReferalUsername);
                            if (referal3 == null) continue;
                            if (campaign.ProcentScore != null) referal3.Cashback += (int)(payment.Amount * campaign.ProcentScore * procents[2] / 10000);
                            if (campaign.Score != null) referal3.Cashback += (int)(campaign.Score * procents[2] / 100);
                        }
                    }
                }


                _db.Users.UpdateRange(users);
                await _db.SaveChangesAsync();

                await _db.Payments.AddRangeAsync(finishPayments);
                await _db.SaveChangesAsync();
                return Ok(finishPayments);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Убедитесь, что вы верно загрузили файлы");
                return View("AddPayments");
            }
            
        }

        [HttpPost("/payment/{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var payment = _db.Payments.Find(id);
            var campaign = _db.AdvertisingСampaigns.FirstOrDefault(x => x.Id == payment.CampaignId);
            var user = _db.Users.FirstOrDefault(x => x.Id == payment.UserId);
            var procents = campaign.Distribution.Split('/').Select(x => int.Parse(x)).ToList();

            var referal = _db.Users.FirstOrDefault(x => x.Login == user.ReferalUsername);
            if (referal != null)
            {
                if (campaign.ProcentScore != null) referal.Cashback -= (int)(payment.Amount * campaign.ProcentScore * procents[0] / 10000);
                if (campaign.Score != null) referal.Cashback -= (int)(campaign.Score * procents[0] / 100);

                if (campaign.ReferalSystem != ReferalSystem.OneLevel)
                {
                    var referal2 = _db.Users.FirstOrDefault(user => user.Login == referal.ReferalUsername);
                    if (referal2 != null)
                    {
                        if (campaign.ProcentScore != null) referal2.Cashback -= (int)(payment.Amount * campaign.ProcentScore * procents[1] / 10000);
                        if (campaign.Score != null) referal2.Cashback -= (int)(campaign.Score * procents[1] / 100);

                        if (campaign.ReferalSystem == ReferalSystem.ThreeLevel)
                        {
                            var referal3 = _db.Users.FirstOrDefault(user => user.Login == referal2.ReferalUsername);
                            if (referal3 == null)
                            {
                                if (campaign.ProcentScore != null) referal3.Cashback -= (int)(payment.Amount * campaign.ProcentScore * procents[2] / 10000);
                                if (campaign.Score != null) referal3.Cashback -= (int)(campaign.Score * procents[2] / 100);
                            }
                        }
                    }
                }
            }
            
            _db.Payments.Remove(payment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
