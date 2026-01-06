using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class VoitureController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VoitureController(ApplicationDbContext context)
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voiture voiture)
        {
            if (ModelState.IsValid)
            {
                _context.Voitures.Add(voiture);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(voiture);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture == null)
            {
                return NotFound();
            }
            return View(voiture);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Voiture voiture)
        {
            var existingVoiture = await _context.Voitures.FindAsync(voiture.CodeVIN);
            if (existingVoiture == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingVoiture.Annee = voiture.Annee;
                existingVoiture.DateAchat = voiture.DateAchat;
                existingVoiture.PrixAchat = voiture.PrixAchat;
                existingVoiture.EnVente = voiture.EnVente;
                existingVoiture.FinitionId = voiture.FinitionId;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(voiture);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture == null)
            {
                return NotFound();
            }
            return View(voiture);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var voiture = await _context.Voitures.FindAsync(id);
            if (voiture == null)
            {
                return NotFound();
            }
            _context.Voitures.Remove(voiture);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}