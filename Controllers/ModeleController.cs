using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace P5_ExpressVoitures.Controllers
{
    public class ModeleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModeleController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var modele = await _context.Modeles
                .Include(m => m.Marque)
                .Include(m => m.Finitions)
                .ToListAsync();
            return View(modele);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var modele = await _context.Modeles.FindAsync(id);
            if (modele == null)
            {
                return NotFound();
            }
            return View(modele);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Modele modele)
        {
            if (ModelState.IsValid)
            {
                _context.Modeles.Add(modele);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(modele);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var modele = await _context.Modeles.FindAsync(id);
            if (modele == null)
            {
                return NotFound();
            }
            return View(modele);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Modele modele)
        {
            if (id != modele.Id) return BadRequest();

            var existingModele = await _context.Modeles.FindAsync(id);
            if (existingModele == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingModele.Nom = modele.Nom;
                existingModele.MarqueId = modele.MarqueId;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(modele);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var modele = await _context.Modeles.FindAsync(id);
            if (modele == null)
            {
                return NotFound();
            }
            return View(modele);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modele = await _context.Modeles.FindAsync(id);
            if (modele == null)
            {
                return NotFound();
            }
            _context.Modeles.Remove(modele);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}