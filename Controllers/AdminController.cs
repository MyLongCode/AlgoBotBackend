using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgoBotBackend.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> AllCourses()
        {
            var courses = _db.Courses.OrderBy(c => c.Name).ToList();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id)
        {
            if (id == null) return NotFound();
            var course = _db.Courses.Find(id);
            if (course == null) return NotFound();
            _db.Courses.Remove(course);
            _db.SaveChanges();
            return RedirectToAction("AllCourses");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourseAsync(CreateCourseViewModel dto)
        {
            var course = new Course()
            {
                Name = dto.Name,
            };
            await _db.AddAsync(course);
            await _db.SaveChangesAsync();
            return RedirectToAction("AllCourses");
        }
    }
}
