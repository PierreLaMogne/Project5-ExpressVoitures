using Microsoft.EntityFrameworkCore;
using Net_P5.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Net_P5.Models
{
    public class VoitureViewModel
    {
        [Required(ErrorMessage = "Le code VIN est obligatoire")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Le code VIN doit contenir exactement 17 caractères alphanumériques (sans les lettres I, O, Q).")]
        [MaxLength(17)]
        public string CodeVIN { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Le prix de vente est obligatoire")]
        [Precision(12, 2)]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal PrixVente { get; set; }
        
        [Required(ErrorMessage = "L'année est obligatoire")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "L'année doit être au format AAAA.")]
        [CurrentYear(1990, ErrorMessage = "L'année doit être comprise entre 1990 et l'année en cours.")]
        public int Annee { get; set; }
        
        [Required(ErrorMessage = "La marque est obligatoire")]
        public int MarqueId { get; set; }
        
        [Required(ErrorMessage = "Le modèle est obligatoire")]
        public int ModeleId { get; set; }
        
        [Required(ErrorMessage = "La finition est obligatoire")]
        public int FinitionId { get; set; }
        
        public IFormFile? Photo { get; set; }
    }
}