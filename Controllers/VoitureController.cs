using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VoitureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public VoitureController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new VoitureViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoitureViewModel model, IFormFile? Photo)
        {
            //Vérification du format des données
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                // Amélioration WCAG : Ajouter un message global accessible
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                return View(model);
            }

            //Vérification d'un doublon en base
            if (await _context.Voitures.AnyAsync(v => v.CodeVIN == model.CodeVIN))
            {
                ModelState.AddModelError("CodeVIN", "Une voiture avec ce code VIN existe déjà.");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            //Vérification du couple Marque-Modele
            var modele = await _context.Modeles.FirstOrDefaultAsync(m => m.Id == model.ModeleId);
            if (modele == null || modele.MarqueId != model.MarqueId)
            {
                ModelState.AddModelError("ModeleId", "Le modèle choisi n'appartient pas à la marque sélectionnée");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            //Vérification du couple Modele-Finition
            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == model.FinitionId);
            if (finition == null || finition.ModeleId != model.ModeleId)
            {
                ModelState.AddModelError("FinitionId", "La finition choisie n'appartient pas au modèle sélectionné");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            // Vérification du fichier photo
            if (model.Photo != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(model.Photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Photo", "Format non autorisé. Utilisez JPG, PNG, GIF ou WebP.");
                    ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                    await PopulateDropdowns();
                    return View(model);
                }

                if (model.Photo.Length > 5 * 1024 * 1024) // 5 Mo
                {
                    ModelState.AddModelError("Photo", "La taille de l'image ne doit pas dépasser 5 Mo.");
                    ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                    await PopulateDropdowns();
                    return View(model);
                }
            }

            //Création de la voiture
            var voiture = new Voiture
            {
                CodeVIN = model.CodeVIN,
                PrixVente = model.PrixVente,
                Annee = model.Annee,
                MarqueId = model.MarqueId,
                ModeleId = model.ModeleId,
                FinitionId = model.FinitionId
            };

            //Création de la photo
            if (Photo != null && Photo.Length > 0)
            {
                voiture.Photo = Photo;
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                voiture.PhotoUrl = "/uploads/" + uniqueFileName;
            }

            //Enregistrement des données et redirection
            _context.Voitures.Add(voiture);
            await _context.SaveChangesAsync();

            // Amélioration WCAG : Message de succès pour les lecteurs d'écran
            TempData["SuccessMessage"] = "La voiture a été créée avec succès.";

            return RedirectToAction(nameof(CreateConfirmation));
        }

        [HttpGet]
        public IActionResult CreateConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != _context.Voitures.FirstOrDefault(v => v.CodeVIN == id)?.CodeVIN)
            {
                return BadRequest();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);

            if (voiture == null)
                return NotFound();

            var viewModel = new VoitureViewModel
            {
                CodeVIN = voiture.CodeVIN,
                PrixVente = voiture.PrixVente,
                Annee = voiture.Annee,
                MarqueId = voiture.MarqueId,
                ModeleId = voiture.ModeleId,
                FinitionId = voiture.FinitionId,
                PhotoUrl = voiture.PhotoUrl
            };

            await PopulateDropdowns();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VoitureViewModel model, IFormFile? Photo)
        {
            //Vérification du format des données
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            //Récupération de la voiture existante
            var voiture = await _context.Voitures.FirstOrDefaultAsync(v => v.CodeVIN == model.CodeVIN);
            if (voiture == null)
            {
                return NotFound();
            }

            //Vérification du couple Marque-Modele
            var modele = await _context.Modeles.FirstOrDefaultAsync(m => m.Id == model.ModeleId);
            if (modele == null || modele.MarqueId != model.MarqueId)
            {
                ModelState.AddModelError("ModeleId", "Le modèle choisi n'appartient pas à la marque sélectionnée");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            //Vérification du couple Modele-Finition
            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == model.FinitionId);
            if (finition == null || finition.ModeleId != model.ModeleId)
            {
                ModelState.AddModelError("FinitionId", "La finition choisie n'appartient pas au modèle sélectionné");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            // Vérification du fichier photo
            if (Photo != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(Photo.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Photo", "Format non autorisé. Utilisez JPG, PNG, GIF ou WebP.");
                    ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                    await PopulateDropdowns();
                    return View(model);
                }

                if (Photo.Length > 5 * 1024 * 1024) // 5 Mo
                {
                    ModelState.AddModelError("Photo", "La taille de l'image ne doit pas dépasser 5 Mo.");
                    ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                    await PopulateDropdowns();
                    return View(model);
                }

                // Suppression de l'ancienne photo si elle existe
                if (!string.IsNullOrEmpty(voiture.PhotoUrl))
                {
                    var oldPhotoPath = Path.Combine(_env.WebRootPath, voiture.PhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                // Sauvegarde de la nouvelle photo
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                voiture.PhotoUrl = "/uploads/" + uniqueFileName;
            }

            //Mise à jour des propriétés de la voiture
            voiture.PrixVente = model.PrixVente;
            voiture.Annee = model.Annee;
            voiture.MarqueId = model.MarqueId;
            voiture.ModeleId = model.ModeleId;
            voiture.FinitionId = model.FinitionId;

            //Enregistrement des modifications et redirection
            _context.Voitures.Update(voiture);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité avec ses nouvelles propriétés de navigation
            var voitureReloaded = await _context.Voitures
                .Include(v => v.Marque)
                .Include(v => v.Modele)
                .Include(v => v.Finition)
                .FirstOrDefaultAsync(v => v.CodeVIN == model.CodeVIN);

            ViewBag.NomModifie = $@"{voitureReloaded!.Marque.Nom} {voitureReloaded.Modele.Nom} {voitureReloaded.Finition.Nom}";

            return View("EditConfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id != _context.Voitures.FirstOrDefault(v => v.CodeVIN == id)?.CodeVIN)
            {
                return BadRequest();
            }

            var voiture = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);
            if (voiture == null)
            {
                return NotFound();
            }
            return View(voiture);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var voiture = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);
            if (voiture == null)
            {
                return NotFound();
            }

            // Suppression de la photo si elle existe
            if (!string.IsNullOrEmpty(voiture.PhotoUrl))
            {
                var photoPath = Path.Combine(_env.WebRootPath, voiture.PhotoUrl.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
            }

            ViewBag.NomSupprime = voiture.NomComplet;

            _context.Voitures.Remove(voiture);
            await _context.SaveChangesAsync();
            return View("DeleteConfirmation");
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