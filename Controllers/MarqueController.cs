using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;
using Net_P5.ViewModels;

namespace Net_P5.Controllers
{
    [Authorize(Roles = "Admin")]
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
            await PopulateDropdowns();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new MarqueViewModel();
            await PopulateDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (MarqueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(model);
            }

            var marque = new Marque
            {
                Nom = model.MarqueNom
            };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            TempData["Message"] = marque.Nom;

            return RedirectToAction(nameof(CreateConfirmation));
        }

        [HttpGet]
        public IActionResult CreateConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != _context.Marques.FirstOrDefault(v => v.Id == id)?.Id)
            {
                return BadRequest();
            }

            var marque = await _context.Marques.FirstOrDefaultAsync(v => v.Id == id);

            if (marque == null)
                return NotFound();

            var viewModel = new MarqueViewModel
            {
                MarqueId = marque.Id,
                MarqueNom = marque.Nom
            };

            await PopulateDropdowns();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MarqueViewModel model)
        {
            // Vérifier que l'ID de la route correspond à l'ID du modèle
            if (id != model.MarqueId)
            {
                return BadRequest("L'ID de la route ne correspond pas à l'ID du modèle.");
            }

            // Vérification du format des données
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            // Récupération de la marque existante (utiliser l'id validé)
            var marque = await _context.Marques.FirstOrDefaultAsync(v => v.Id == id);
            if (marque == null)
            {
                return NotFound();
            }

            // Mise à jour des propriétés de la marque
            marque.Nom = model.MarqueNom;
                      
            // Enregistrement des modifications et redirection
            _context.Marques.Update(marque);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité
            var marqueReloaded = await _context.Marques.FirstOrDefaultAsync(m => m.Id == id);

            TempData["Message"] = marqueReloaded!.Nom;

            return RedirectToAction(nameof(EditConfirmation));
        }

        [HttpGet]
        public IActionResult EditConfirmation()
        {
            return View();
        }

        //Non nécessaire car utilisation d'un modal
        /*[HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != _context.Marques.FirstOrDefault(m => m.Id == id)?.Id)
            {
                return BadRequest();
            }

            var marque = await _context.Marques.FirstOrDefaultAsync(m => m.Id == id);
            if (marque == null)
            {
                return NotFound();
            }
            
            var viewModel = new MarqueViewModel
            {
                MarqueId = marque.Id,
                MarqueNom = marque.Nom
            };
            
            return View(viewModel);
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id != _context.Marques.FirstOrDefault(m => m.Id == id)?.Id)
            {
                return BadRequest();
            }

            var marque = await _context.Marques.FirstOrDefaultAsync(m => m.Id == id);
            if (marque == null)
            {
                return NotFound();
            }

            //Vérifier qu'aucune voiture n'utilise cette marque avant de la supprimer
            var count = await _context.Voitures
                .Include(v => v.Finition)
                    .ThenInclude(f => f.Modele)
                        .ThenInclude(m => m.Marque)
                .CountAsync(v => v.Finition.Modele.Marque.Id == id);
            if (count > 0)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer cette marque, {count} voiture(s) y sont associée(s).";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = marque.Nom;

            try
            {
                _context.Marques.Remove(marque);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DeleteConfirmation));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la suppression de la marque. Veuillez réessayer.";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpGet]
        public IActionResult DeleteConfirmation()
        {
            return View();
        }

        //Remplissage des champs Select
        public async Task PopulateDropdowns()
        {
            ViewBag.Marques = await _context.Marques.ToListAsync();
            ViewBag.Modeles = await _context.Modeles.ToListAsync();
            ViewBag.Finitions = await _context.Finitions.ToListAsync();
        }

    }
}
