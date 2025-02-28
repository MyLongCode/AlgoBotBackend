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
        public async Task<IActionResult> AllUsers()
        {
            var users = _db.Users.Where(c => c.Role == "user").ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCourse()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(CreateUserViewModel dto)
        {
            var user = new User()
            {
                Role = "user",
                Login = dto.Login,
                Password = dto.Password,
                ReferalUsername = "",
                FullName = dto.FullName,
                PhoneNumber = "",
                ChildAge = "",
                ChildName = "",
                Cashback = 0,
                Score = 0,
                StageReg = 5,
            };
            await _db.AddAsync(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("AllUsers");
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
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (id == null) return NotFound();
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("AllUsers");
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
