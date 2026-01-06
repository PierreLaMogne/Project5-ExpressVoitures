using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class ReparationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReparationController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reparation = await _context.Reparations
                .Include(r => r.Voiture)
                .ToListAsync();
            return View(reparation);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var reparation = await _context.Reparations.FindAsync(id);
            if (reparation == null)
            {
                return NotFound();
            }
            return View(reparation);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reparation reparation)
        {
            if (ModelState.IsValid)
            {
                _context.Reparations.Add(reparation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(reparation);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reparation = await _context.Reparations.FindAsync(id);
            if (reparation == null)
            {
                return NotFound();
            }
            return View(reparation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reparation reparation)
        {
            if (id != reparation.Id) return BadRequest();

            var existingReparation = await _context.Reparations.FindAsync(id);
            if (existingReparation == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingReparation.Detail = reparation.Detail;
                existingReparation.Cout = reparation.Cout;
                existingReparation.DateDisponibilite = reparation.DateDisponibilite;
                existingReparation.VoitureCodeVIN = reparation.VoitureCodeVIN;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(reparation);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var reparation = await _context.Reparations.FindAsync(id);
            if (reparation == null)
            {
                return NotFound();
            }
            return View(reparation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reparation = await _context.Reparations.FindAsync(id);
            if (reparation == null)
            {
                return NotFound();
            }
            _context.Reparations.Remove(reparation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
