using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

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
        public async Task<IActionResult> Create()
        {
            var model = new VoitureViewModel();
            await PopulateDropdowns();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoitureViewModel model)
        {
            if (!ModelState.IsValid)
            {
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

            TempData["Message"] = modele.NomComplet;

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

            var viewModel = new VoitureViewModel
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
        public async Task<IActionResult> Edit(VoitureViewModel model)
        {
            //Vérification du format des données
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            //Récupération du modèle existant
            var modele = await _context.Modeles
                .Include(mo => mo.Marque)
                .FirstOrDefaultAsync(mo => mo.Id == model.ModeleId);
            if (modele == null)
            {
                return NotFound();
            }

            //Mise à jour des propriétés du modèle
            modele.Nom = model.ModeleNom;
            modele.MarqueId = model.MarqueId;

            //Enregistrement des modifications et redirection
            _context.Modeles.Update(modele);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité avec ses nouvelles propriétés de navigation
            var modeleReloaded = await _context.Modeles.FirstOrDefaultAsync(mo => mo.Id == model.ModeleId);

            TempData["Message"] = modeleReloaded!.NomComplet;

            return RedirectToAction(nameof(EditConfirmation));
        }

        [HttpGet]
        public IActionResult EditConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != _context.Modeles.FirstOrDefault(mo => mo.Id == id)?.Id)
            {
                return BadRequest();
            }

            var modele = await _context.Modeles.FirstOrDefaultAsync(mo => mo.Id == id);
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
            if (id != _context.Modeles.FirstOrDefault(mo => mo.Id == id)?.Id)
            {
                return BadRequest();
            }

            var modele = await _context.Modeles.FirstOrDefaultAsync(mo => mo.Id == id);
            if (modele == null)
            {
                return NotFound();
            }

            TempData["Message"] = modele.NomComplet;

            _context.Modeles.Remove(modele);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeleteConfirmation));
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
