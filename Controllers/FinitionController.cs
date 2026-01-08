using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;
using Net_P5.ViewModels;

namespace Net_P5.Controllers
{
    [Authorize(Roles = "Admin")]
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
            await PopulateDropdowns();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new FinitionViewModel();
            await PopulateDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinitionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(model);
            }

            var finition = new Finition
            {
                Nom = model.FinitionNom,
                ModeleId = model.ModeleId
            };

            _context.Finitions.Add(finition);
            await _context.SaveChangesAsync();

            // Charger la Finition pour l'affichage TempData
            var finitionCreee = await _context.Finitions
                .Include(f => f.Modele.Marque)
                .FirstOrDefaultAsync(f => f.Id == finition.Id);

            TempData["Message"] = finition.NomComplet;

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
            if (id != _context.Finitions.FirstOrDefault(f => f.Id == id)?.Id)
            {
                return BadRequest();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (finition == null)
                return NotFound();

            var viewModel = new FinitionViewModel
            {
                FinitionId = finition.Id,
                FinitionNom = finition.Nom,
                ModeleId = finition.Modele.Id
            };

            await PopulateDropdowns();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FinitionViewModel model)
        {
            // Vérifier que l'ID de la route correspond à l'ID du modèle
            if (id != model.FinitionId)
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

            // Récupération de la finition existante (utiliser l'id validé)
            var finition = await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(f => f.Id == id);
                
            if (finition == null)
            {
                return NotFound();
            }

            // Mise à jour des propriétés de la finition
            finition.Nom = model.FinitionNom;
            finition.ModeleId = model.ModeleId;

            // Enregistrement des modifications et redirection
            _context.Finitions.Update(finition);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité avec ses nouvelles propriétés de navigation
            var finitionReloaded = await _context.Finitions
                .Include(f => f.Modele)
                    .ThenInclude(m => m.Marque)
                .FirstOrDefaultAsync(f => f.Id == id);

            TempData["Message"] = finitionReloaded!.NomComplet;

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
            if (id != _context.Finitions.FirstOrDefault(f => f.Id == id)?.Id)
            {
                return BadRequest();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(f => f.Id == id);
                
            if (finition == null)
            {
                return NotFound();
            }
            
            var viewModel = new FinitionViewModel
            {
                FinitionId = finition.Id,
                FinitionNom = finition.Nom,
                ModeleId = finition.Modele.Id
            };
            
            return View(viewModel);
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id != _context.Finitions.FirstOrDefault(f => f.Id == id)?.Id)
            {
                return BadRequest();
            }

            var finition = await _context.Finitions
                .Include(f => f.Modele.Marque)
                .FirstOrDefaultAsync(f => f.Id == id);
                
            if (finition == null)
            {
                return NotFound();
            }

            //Vérifier qu'aucune voiture n'utilise cette finition avant de la supprimer
            var count = await _context.Voitures
                .Include(v => v.Finition)
                    .ThenInclude(f => f.Modele)
                        .ThenInclude(m => m.Marque)
                .CountAsync(v => v.Finition.Id == id);
            if (count > 0)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer cette finition, {count} voiture(s) y sont associée(s).";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = finition.Nom;

            try
            {
                _context.Finitions.Remove(finition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DeleteConfirmation));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la suppression de la finition. Veuillez réessayer.";
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
