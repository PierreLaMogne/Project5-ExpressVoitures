using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class FinitionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FinitionController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var finitions = await _context.Finitions
                .Include(f => f.Modele)
                .ToListAsync();
            return View(finitions);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var finition = await _context.Finitions.FindAsync(id);
            if (finition == null)
            {
                return NotFound();
            }
            return View(finition);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Finition finition)
        {
            if (ModelState.IsValid)
            {
                _context.Finitions.Add(finition);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(finition);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var finition = await _context.Finitions.FindAsync(id);
            if (finition == null)
            {
                return NotFound();
            }
            return View(finition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Finition finition)
        {
            if (id != finition.Id) return BadRequest();

            var existingFinition = await _context.Finitions.FindAsync(id);
            if (existingFinition == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                existingFinition.Nom = finition.Nom;
                existingFinition.ModeleId = finition.ModeleId;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(finition);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var finition = await _context.Finitions.FindAsync(id);
            if (finition == null)
            {
                return NotFound();
            }
            return View(finition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var finition = await _context.Finitions.FindAsync(id);
            if (finition == null)
            {
                return NotFound();
            }
            _context.Finitions.Remove(finition);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
