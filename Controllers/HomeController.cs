using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var voitures = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .Include(v => v.Reparations)
                .ToListAsync();
            return View(voitures);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var voiture = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .Include(v => v.Reparations)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);
            if (voiture == null)
            {
                return NotFound();
            }
            return View(voiture);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voiture voiture)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(voiture);
            }

            if (await _context.Voitures.AnyAsync(v => v.MarqueId != voiture.Modele.MarqueId))
            {
                ModelState.AddModelError(string.Empty, "Le modèle choisi n'appartient pas à la marque sélectionnée");
                await PopulateDropdowns();
                return View(voiture);
            }

            if (await _context.Voitures.AnyAsync(v => v.ModeleId != voiture.Finition.ModeleId))
            {
                ModelState.AddModelError(string.Empty, "La finition choisie n'appartient pas au modèle sélectionné");
                await PopulateDropdowns();
                return View(voiture);
            }

            if (voiture.Photo != null && voiture.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + voiture.Photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await voiture.Photo.CopyToAsync(fileStream);
                }
                voiture.PhotoUrl = "/uploads/" + uniqueFileName;
            }

            _context.Add(voiture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CreateConfirmation));
        }

        [HttpGet]
        public IActionResult CreateConfirmation()
        {
            return View();
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

        public async Task PopulateDropdowns()
        {
            ViewBag.Marques = await _context.Marques.ToListAsync();
            ViewBag.Modeles = await _context.Modeles.ToListAsync();
            ViewBag.Finitions = await _context.Finitions.ToListAsync();
        }
    }
}
