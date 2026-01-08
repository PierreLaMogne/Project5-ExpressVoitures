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
            var model = new VoitureViewModel
            {
                DateAchat = DateOnly.FromDateTime(DateTime.Now),
                DateDisponibilite = DateOnly.FromDateTime(DateTime.Now),
            };

            await PopulateDropdowns();
            return View(model);
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

            //Vérification des dates
            if (model.VoitureVendue == true && !model.DateVente.HasValue)
            {
                ModelState.AddModelError("DateVente", "La date de vente est requise si la voiture est vendue");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            if (model.DateVente.HasValue && model.VoitureVendue == true)
            {
                if (model.DateVente < model.DateDisponibilite)
                {
                    ModelState.AddModelError("DateVente", "La date de vente est antérieure à la date de disponibilité");
                    ModelState.AddModelError("DateDisponibilite", "La date de disponibilité est postérieure à la date de vente");
                    ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                    await PopulateDropdowns();
                    return View(model);
                }
            }

            if (model.DateDisponibilite < model.DateAchat)
            {
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité est antérieure à la date d'achat");
                ModelState.AddModelError("DateAchat", "La date d'achat est postérieure à la date de disponibilité");
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
                Annee = model.Annee,
                DateAchat = model.DateAchat,
                PrixAchat = model.PrixAchat,
                EnVente = model.EnVente,
                FinitionId = model.FinitionId,
            };

            //Création de la réparation
            var reparation = new Reparation
            {
                Detail = model.Detail,
                Cout = model.Cout,
                DateDisponibilite = model.DateDisponibilite
            };
            voiture.Reparations = new List<Reparation> { reparation };

            //Création de la vente si applicable
            if (model.DateVente.HasValue && model.VoitureVendue == true)
            {
                var vente = new Vente
                {
                    DateVente = model.DateVente.Value
                };
                voiture.Ventes = new List<Vente> { vente };
            }

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

            TempData["Message"] = voiture.NomComplet;

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
                .Include(v => v.Reparations)
                .Include(v => v.Ventes)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);

            if (voiture == null)
                return NotFound();

            // Récupérer la dernière réparation et la dernière vente si elles existent
            var reparation = voiture.Reparations?.OrderByDescending(r => r.DateDisponibilite).FirstOrDefault();
            var vente = voiture.Ventes?.OrderByDescending(v => v.DateVente).FirstOrDefault();

            var viewModel = new VoitureViewModel
            {
                CodeVIN = voiture.CodeVIN,
                Annee = voiture.Annee,
                DateAchat = voiture.DateAchat,
                PrixAchat = voiture.PrixAchat,
                EnVente = voiture.EnVente,
                MarqueId = voiture.Finition.Modele.MarqueId,
                ModeleId = voiture.Finition.ModeleId,
                FinitionId = voiture.FinitionId,
                PhotoUrl = voiture.PhotoUrl
            };

            // Attribuer les propriétés de réparation uniquement si une réparation existe
            if (reparation != null)
            {
                viewModel.Detail = reparation.Detail;
                viewModel.Cout = reparation.Cout;
                viewModel.DateDisponibilite = reparation.DateDisponibilite;
            }

            // Attribuer les propriétés de vente uniquement si une vente existe
            if (vente != null)
            {
                viewModel.DateVente = vente.DateVente;
            }

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

            //Vérification des dates
            if (model.VoitureVendue == true && !model.DateVente.HasValue)
            {
                ModelState.AddModelError("DateVente", "La date de vente est requise si la voiture est vendue");
                ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                await PopulateDropdowns();
                return View(model);
            }

            if (model.DateVente.HasValue && model.VoitureVendue == true)
            {
                if(model.DateVente < model.DateDisponibilite)
                {
                    ModelState.AddModelError("DateVente", "La date de vente est antérieure à la date de disponibilité");
                    ModelState.AddModelError("DateDisponibilite", "La date de disponibilité est postérieure à la date de vente");
                    ViewBag.ErrorSummary = "Le formulaire contient des erreurs. Veuillez corriger les champs indiqués ci-dessous.";
                    await PopulateDropdowns();
                    return View(model);
                }
            }

            if (model.DateDisponibilite < model.DateAchat)
            {
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité est antérieure à la date d'achat");
                ModelState.AddModelError("DateAchat", "La date d'achat est postérieure à la date de disponibilité");
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
            voiture.CodeVIN = model.CodeVIN;
            voiture.Annee = model.Annee;
            voiture.DateAchat = model.DateAchat;
            voiture.PrixAchat = model.PrixAchat;
            voiture.EnVente = model.EnVente;
            voiture.FinitionId = model.FinitionId;

            //Mise à jour des réparations associées si nécessaire
            var reparation = await _context.Reparations
                .Where(r => r.VoitureCodeVIN == voiture.CodeVIN)
                .OrderByDescending(r => r.DateDisponibilite)
                .FirstOrDefaultAsync();

            if (reparation != null)
            {
                reparation.Detail = model.Detail;
                reparation.Cout = model.Cout;
                reparation.DateDisponibilite = model.DateDisponibilite;
            }

            //Mise à jour ou création des ventes associées si nécessaire
            var vente = await _context.Ventes
                .Where(v => v.VoitureCodeVIN == voiture.CodeVIN)
                .OrderByDescending(v => v.DateVente)
                .FirstOrDefaultAsync();

            if (vente == null && model.DateVente.HasValue && model.VoitureVendue == true)
            {
                var venteCreee = new Vente
                {
                    DateVente = model.DateVente.Value
                };
                voiture.Ventes = new List<Vente> { venteCreee };
            }

            if (vente != null && model.DateVente.HasValue && model.VoitureVendue == true)
            {
                vente.DateVente = model.DateVente.Value;
            }

            if (vente != null && model.VoitureVendue == false)
            {
                _context.Ventes.Remove(vente);
            }

            //Enregistrement des modifications et redirection
            _context.Voitures.Update(voiture);
            await _context.SaveChangesAsync();

            // Recharger complètement l'entité avec ses nouvelles propriétés de navigation
            var voitureReloaded = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .FirstOrDefaultAsync(v => v.CodeVIN == model.CodeVIN);

            TempData["Message"] = voitureReloaded!.NomComplet;

            return RedirectToAction(nameof(EditConfirmation));
        }

        [HttpGet]
        public IActionResult EditConfirmation()
        {
            return View();
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

            TempData["Message"] = voiture.NomComplet;

            _context.Voitures.Remove(voiture);
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