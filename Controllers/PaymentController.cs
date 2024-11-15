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
        public async Task<IActionResult> AddPayments()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayments(AddPaymentViewModel dto)
        {
            if (dto.Payments == null) return StatusCode(400);

            

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
                s.Phonenumber = s.Phonenumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Replace(" ", "").Replace("+","");
                return s;
            })
                .Where(s => users.FirstOrDefault(p => p.PhoneNumber == s.Phonenumber) != null)
                .ToList();



            var studentPayments = payments
                .Where(s => students.FirstOrDefault(p => p.StudentId == s.StudentId) != null)
                .Where(s => campaigns.Where(c => c.Courses.Select(x => x.Name).Contains(s.CourseName)) != null)
                .Select(s => new StudentPaymentsCSV
            {
                Amount = (int) s.Amount,
                Campaign = campaigns.FirstOrDefault(c => c.Courses.Select(x => x.Name).Contains(s.CourseName)),
                PhoneNumber = students.FirstOrDefault( x => x.StudentId == s.StudentId).Phonenumber
            }).ToList();



            var finishPayments = studentPayments
                .Where(s => s.Campaign != null)
                .Select(s => new Payment()
            {
                UserId = users.FirstOrDefault(u => u.PhoneNumber == s.PhoneNumber).Id,
                Amount = s.Amount,
                CampaignId = s.Campaign.Id,
            }).ToList();
            
            foreach(var user in users)
                user.Score += finishPayments.Where(p => p.UserId == user.Id).Sum(p => p.Amount);

            _db.Users.UpdateRange(users);
            await _db.SaveChangesAsync();

            await _db.Payments.AddRangeAsync(finishPayments);
            await _db.SaveChangesAsync();

            return Ok(finishPayments);
        }
    }
}
