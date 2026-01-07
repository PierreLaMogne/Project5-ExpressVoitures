using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;
using System.Diagnostics;

namespace Net_P5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var voitures = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .Include(v => v.Reparations)
                .Include(v => v.Ventes)
                .ToListAsync();

            return View(voitures);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var voiture = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .Include(v => v.Reparations)
                .Include(v => v.Ventes)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);

            if (voiture == null)
            {
                return NotFound();
            }
            return View(voiture);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
