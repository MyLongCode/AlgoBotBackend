using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.CSV;
using AlgoBotBackend.Models.ViewModels;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var payments = csvPayments.GetRecords<PaymentCSV>();
            var students = csvStudents.GetRecords<StudentCSV>();
            var pattern = "[-., ({})@#$%^&*!+=:;/\\[\\]]+";
            var rgx = new Regex(pattern);
            students = students.Select(s => 
            {
                s.Phonenumber = rgx.Replace(s.Phonenumber, "");
                return s;
            }).ToList();
            return Ok(students);
        }
    }
}
