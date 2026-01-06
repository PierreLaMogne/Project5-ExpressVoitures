using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class MarqueController : Controller
        {
        private readonly ApplicationDbContext _context;
        public MarqueController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var marque = await _context.Marques
                .Include(m => m.Modeles)
                    .ThenInclude(mo => mo.Finitions)
                .ToListAsync();
            return View(marque);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var marque = await _context.Marques.FindAsync(id);
            if (marque == null)
            {
                return NotFound();
            }
            return View(marque);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marque marque)
        {
            if (ModelState.IsValid)
            {
                _context.Marques.Add(marque);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(marque);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var marque = await _context.Marques.FindAsync(id);
            if (marque == null)
            {
                return NotFound();
            }
            return View(marque);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marque marque)
        {
            if (id != marque.Id) return BadRequest();

            var existingMarque = await _context.Marques.FindAsync(id);
            if (existingMarque == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingMarque.Nom = marque.Nom;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(marque);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var marque = await _context.Marques.FindAsync(id);
            if (marque == null)
            {
                return NotFound();
            }
            return View(marque);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marque = await _context.Marques.FindAsync(id);
            if (marque == null)
            {
                return NotFound();
            }
            _context.Marques.Remove(marque);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
