using Microsoft.EntityFrameworkCore;
using Net_P5.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.ViewModels
{
    public class ModeleViewModel
    {
        [Required(ErrorMessage = "La marque du véhicule est obligatoire")]
        public int MarqueId { get; set; }

        [Required(ErrorMessage = "Le modèle du véhicule est obligatoire")]
        public int ModeleId { get; set; }

        [Required(ErrorMessage = "Le nom du modèle est obligatoire")]
        [MaxLength(50, ErrorMessage = "Le nom du modèle ne peut pas dépasser 50 caractères.")]
        public string ModeleNom { get; set; } = string.Empty;
    }
}