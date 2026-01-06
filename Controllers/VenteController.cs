using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class VenteController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VenteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ventes = await _context.Ventes
                .Include(v => v.Voiture)
                .ToListAsync();
            return View(ventes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }
            return View(vente);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vente vente)
        {
            if (ModelState.IsValid)
            {
                _context.Ventes.Add(vente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(vente);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }
            return View(vente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vente vente)
        {
            if (id != vente.Id) return BadRequest();

            var existingVente = await _context.Ventes.FindAsync(id);
            if (existingVente == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingVente.DateVente = vente.DateVente;
                existingVente.VoitureCodeVIN = vente.VoitureCodeVIN;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(vente);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }
            return View(vente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }
            _context.Ventes.Remove(vente);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
