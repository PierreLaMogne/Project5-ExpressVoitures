using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

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

            var finition = new Finition
            {
                Nom = model.FinitionNom,
                ModeleId = model.ModeleId
            };

            _context.Finitions.Add(finition);
            await _context.SaveChangesAsync();

            // Charger la Finition avec ses relations pour l'affichage
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

            var viewModel = new VoitureViewModel
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
        public async Task<IActionResult> Edit(VoitureViewModel model)
        {
            //Vérification du format des données
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            //Récupération de la finition existante
            var finition = await _context.Finitions
                .Include(f => f.Modele)
                .FirstOrDefaultAsync(f => f.Id == model.FinitionId);
            if (finition == null)
            {
                return NotFound();
            }
            if (model.FinitionId != _context.Finitions.FirstOrDefault(f => f.Id == model.FinitionId)?.Id)
            {
                return BadRequest();
            }

            //Mise à jour des propriétés de la finition
            finition.Nom = model.FinitionNom;
            finition.ModeleId = model.ModeleId;

            //Enregistrement des modifications et redirection
            _context.Finitions.Update(finition);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité avec ses nouvelles propriétés de navigation
            var finitionReloaded = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == model.FinitionId);

            TempData["Message"] = finitionReloaded!.NomComplet;

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
            if (id != _context.Finitions.FirstOrDefault(f => f.Id == id)?.Id)
            {
                return BadRequest();
            }

            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == id);
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
            if (id != _context.Finitions.FirstOrDefault(f => f.Id == id)?.Id)
            {
                return BadRequest();
            }

            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == id);
            if (finition == null)
            {
                return NotFound();
            }

            TempData["Message"] = finition.NomComplet;

            _context.Finitions.Remove(finition);
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
