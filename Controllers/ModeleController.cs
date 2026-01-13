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

            //Vérification si un modèle porte déjà ce nom
            var modeleExist = await _context.Modeles
                .FirstOrDefaultAsync(mo => mo.Nom == model.ModeleNom);
            if (modeleExist != null)
            {
                ModelState.AddModelError("ModeleNom", "Un modèle avec ce nom existe déjà.");
                await PopulateDropdowns();
                return View(model);
            }


            var modele = new Modele
            {
                Nom = model.ModeleNom,
                MarqueId = model.MarqueId
            };

            _context.Modeles.Add(modele);
            await _context.SaveChangesAsync();

            // Charger le modèle pour l'affichage TempData
            var modeleCree = await _context.Modeles
                .Include(m => m.Marque)
                .FirstOrDefaultAsync(m => m.Nom == modele.Nom);

            TempData["Message"] = modeleCree!.NomComplet;

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
                TempData["ErrorMessage"] = "Le nom saisi pour le modèle est incorrect (nom obligatoire avec 50 caractères max)";
                await PopulateDropdowns();
                return RedirectToAction(nameof(Index));
            }

            //Vérification si un modèle porte déjà ce nom (autre que celle en cours de modification)
            var modeleExist = await _context.Modeles
                .FirstOrDefaultAsync(mo => mo.Nom == model.ModeleNom && mo.Id != id);
            if (modeleExist != null)
            {
                TempData["ErrorMessage"] = "Un modèle avec ce nom existe déjà";
                await PopulateDropdowns();
                return RedirectToAction(nameof(Index));
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
                .FirstOrDefaultAsync(mo => mo.Id == modele.Id);

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

            TempData["Message"] = modele.NomComplet;

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
            ViewBag.Marques = await _context.Marques.OrderBy(m => m.Nom).ToListAsync();
            ViewBag.Modeles = await _context.Modeles.OrderBy(mo => mo.Nom).ToListAsync();
            ViewBag.Finitions = await _context.Finitions.OrderBy(f => f.Modele.Nom).ThenBy(f => f.Nom).ToListAsync();
        }

    }
}
