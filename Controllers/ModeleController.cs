using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;
using Net_P5.ViewModels;

namespace Net_P5.Controllers
{
    [Authorize(Roles = "Admin")]
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
            await PopulateDropdowns();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ModeleViewModel();
            await PopulateDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ModeleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(model);
            }

            var modelee = new Modele
            {
                Nom = model.ModeleNom,
                MarqueId = model.MarqueId
            };

            _context.Modeles.Add(modelee);
            await _context.SaveChangesAsync();

            // Charger le modèle pour l'affichage TempData
            var modeleReloaded = await _context.Modeles
                .Include(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Id == modelee.Id);

            TempData["Message"] = modeleReloaded!.NomComplet;

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
            if (id != _context.Modeles.FirstOrDefault(mo => mo.Id == id)?.Id)
            {
                return BadRequest();
            }

            var modele = await _context.Modeles
                .Include(mo => mo.Marque)
                .FirstOrDefaultAsync(mo => mo.Id == id);

            if (modele == null)
                return NotFound();

            var viewModel = new ModeleViewModel
            {
                ModeleId = modele.Id,
                ModeleNom = modele.Nom,
                MarqueId = modele.Marque.Id,
                MarqueNom = modele.Marque.Nom
            };

            await PopulateDropdowns();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ModeleViewModel model)
        {
            // Vérifier que l'ID de la route correspond à l'ID du modèle
            if (id != model.ModeleId)
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

            // Récupération du modèle existant (utiliser l'id validé)
            var modele = await _context.Modeles
                .Include(mo => mo.Marque)
                .FirstOrDefaultAsync(mo => mo.Id == id);
            if (modele == null)
            {
                return NotFound();
            }

            // Mise à jour des propriétés du modèle
            modele.Nom = model.ModeleNom;
            modele.MarqueId = model.MarqueId;

            // Enregistrement des modifications et redirection
            _context.Modeles.Update(modele);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité avec ses nouvelles propriétés de navigation
            var modeleReloaded = await _context.Modeles
                .Include(mo => mo.Marque)
                .FirstOrDefaultAsync(mo => mo.Id == id);

            TempData["Message"] = modeleReloaded!.NomComplet;

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
            if (id != _context.Modeles.FirstOrDefault(mo => mo.Id == id)?.Id)
            {
                return BadRequest();
            }

            var modele = await _context.Modeles
                .Include(mo => mo.Marque)
                .FirstOrDefaultAsync(mo => mo.Id == id);
                
            if (modele == null)
            {
                return NotFound();
            }
            
            var viewModel = new ModeleViewModel
            {
                ModeleId = modele.Id,
                ModeleNom = modele.Nom,
                MarqueId = modele.Marque.Id,
                MarqueNom = modele.Marque.Nom
            };
            
            return View(viewModel);
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id != _context.Modeles.FirstOrDefault(mo => mo.Id == id)?.Id)
            {
                return BadRequest();
            }

            var modele = await _context.Modeles
                .Include(mo => mo.Marque)
                .FirstOrDefaultAsync(mo => mo.Id == id);
                
            if (modele == null)
            {
                return NotFound();
            }

            //Vérifier qu'aucune voiture n'utilise ce modèle avant de la supprimer
            var count = await _context.Voitures
                .Include(v => v.Finition)
                    .ThenInclude(f => f.Modele)
                        .ThenInclude(m => m.Marque)
                .CountAsync(v => v.Finition.Modele.Id == id);
            if (count > 0)
            {
                TempData["ErrorMessage"] = $"Impossible de supprimer ce modèle, {count} voiture(s) y sont associée(s).";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = modele.Nom;

            try
            {
                _context.Modeles.Remove(modele);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DeleteConfirmation));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de la suppression du modèle. Veuillez réessayer.";
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
