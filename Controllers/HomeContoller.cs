using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlgoBotBackend.Migrations.DAL;
using AlgoBotBackend.Migrations.EF;

namespace AlgoBotBackend.Controllers
{
    public class HomeContoller : Controller
    {
        private readonly DBContext _context;

        public HomeContoller(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.BotUsers.ToListAsync());
        }

        private bool BotUserExists(string id)
        {
            return _context.BotUsers.Any(e => e.Username == id);
        }
    }
}
