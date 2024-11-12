using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;
using AlgoBotBackend.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlgoBotBackend.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger, DBContext db)
        {
            _logger = logger;
            _db = db;
        }
        
      
    }
}
