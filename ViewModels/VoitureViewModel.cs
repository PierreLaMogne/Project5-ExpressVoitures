using Microsoft.EntityFrameworkCore;
using Net_P5.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.ViewModels
{
    public class VoitureViewModel
    {
        //Données de la voiture
        [Required(ErrorMessage = "Le code VIN est obligatoire")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Le code VIN doit contenir exactement 17 caractères alphanumériques (sans les lettres I, O, Q).")]
        [MaxLength(17)]
        public string CodeVIN { get; set; } = string.Empty;
                
        [Required(ErrorMessage = "L'année est obligatoire")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "L'année doit être au format AAAA.")]
        [CurrentYear(1990, ErrorMessage = "L'année doit être comprise entre 1990 et l'année en cours.")]
        public int Annee { get; set; }
                
        [Required(ErrorMessage = "La date d'achat est obligatoire.")]
        public DateOnly DateAchat { get; set; }

        [Required(ErrorMessage = "Le prix d'achat est obligatoire.")]
        [Precision(12, 2)]
        [Range(0, (double)decimal.MaxValue)]
        public decimal PrixAchat { get; set; }

        [Required(ErrorMessage = "Veuillez indiquer si la voiture est en vente")]
        public bool EnVente { get; set; } = false;


        //Données du type de véhicule
        [Required(ErrorMessage = "La marque du véhicule est obligatoire")]
        public int MarqueId { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom de la marque ne peut pas dépasser 50 caractères.")]
        public string MarqueNom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le modèle du véhicule est obligatoire")]
        public int ModeleId { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom du modèle ne peut pas dépasser 50 caractères.")]
        public string ModeleNom { get; set; } = string.Empty;

        [Required(ErrorMessage = "La finition du véhicule est obligatoire")]
        public int FinitionId { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom de la finition ne peut pas dépasser 50 caractères.")]
        public string FinitionNom { get; set; } = string.Empty;


        //Données des réparations
        [Required(ErrorMessage = "Le détail de la réparation est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le détail de la réparation ne peut pas dépasser 200 caractères.")]
        public string Detail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le coût de la réparation est obligatoire.")]
        [Precision(12, 2)]
        [Range(0, (double)decimal.MaxValue)]
        public decimal Cout { get; set; }

        [Required(ErrorMessage = "La date de disponibilité est obligatoire.")]
        public DateOnly DateDisponibilite { get; set; }

        //Données de la vente
        public bool VoitureVendue { get; set; }
        public DateOnly? DateVente { get; set; }


        //Données de la photo
        public IFormFile? Photo { get; set; }
        public string? PhotoUrl { get; set; }

    }
}