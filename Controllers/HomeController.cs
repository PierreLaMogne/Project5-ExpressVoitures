using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Net_P5.Data;
using Net_P5.Models;

namespace Net_P5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public HomeController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var voitures = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .ToListAsync();
            return View(voitures);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var voiture = await _context.Voitures
                .Include(v => v.Finition.Modele.Marque)
                .FirstOrDefaultAsync(v => v.CodeVIN == id);
            if (voiture == null)
            {
                return NotFound();
            }
            return View(voiture);
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
                return View(model);
            }

            //Vérification d'un doublon en base
            if (await _context.Voitures.AnyAsync(v => v.CodeVIN == model.CodeVIN))
            {
                ModelState.AddModelError("CodeVIN", "Une voiture avec ce code VIN existe déjà.");
                await PopulateDropdowns();
                return View(model);
            }

            //Vérification du couple Marque-Modele
            var modele = await _context.Modeles.FirstOrDefaultAsync(m => m.Id == model.ModeleId);
            if (modele == null || modele.MarqueId != model.MarqueId)
            {
                ModelState.AddModelError(string.Empty, "Le modèle choisi n'appartient pas à la marque sélectionnée");
                await PopulateDropdowns();
                return View(model);
            }

            //Vérification du couple Modele-Finition
            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == model.FinitionId);
            if (finition == null || finition.ModeleId != model.ModeleId)
            {
                ModelState.AddModelError(string.Empty, "La finition choisie n'appartient pas au modèle sélectionné");
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
                }

                if (model.Photo.Length > 5 * 1024 * 1024) // 5 Mo
                {
                    ModelState.AddModelError("Photo", "La taille de l'image ne doit pas dépasser 5 Mo.");
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
                var filePath = Path.Combine(uploadsFolder, Photo.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                voiture.PhotoUrl = "/uploads/" + Photo.FileName;
            }

            //Enregistrement des données et redirection
            _context.Voitures.Add(voiture);
            await _context.SaveChangesAsync();
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
                ModelState.AddModelError(string.Empty, "Le modèle choisi n'appartient pas à la marque sélectionnée");
                await PopulateDropdowns();
                return View(model);
            }

            //Vérification du couple Modele-Finition
            var finition = await _context.Finitions.FirstOrDefaultAsync(f => f.Id == model.FinitionId);
            if (finition == null || finition.ModeleId != model.ModeleId)
            {
                ModelState.AddModelError(string.Empty, "La finition choisie n'appartient pas au modèle sélectionné");
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
                    await PopulateDropdowns();
                    return View(model);
                }

                if (Photo.Length > 5 * 1024 * 1024) // 5 Mo
                {
                    ModelState.AddModelError("Photo", "La taille de l'image ne doit pas dépasser 5 Mo.");
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
                var uniqueFileName = $"{Guid.NewGuid()}_{Photo.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                voiture.PhotoUrl = "/uploads/" + uniqueFileName;
            }

            //Mise à jour des propriétés de la voiture
            voiture.CodeVIN = model.CodeVIN;
            voiture.PrixVente = model.PrixVente;
            voiture.Annee = model.Annee;
            voiture.MarqueId = model.MarqueId;
            voiture.ModeleId = model.ModeleId;
            voiture.FinitionId = model.FinitionId;

            //Enregistrement des modifications et redirection
            _context.Voitures.Update(voiture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = voiture.CodeVIN});
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
