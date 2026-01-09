using Microsoft.EntityFrameworkCore;
using Net_P5.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.ViewModels
{
    public class FinitionViewModel
    {
        [Required(ErrorMessage = "Le modèle du véhicule est obligatoire")]
        public int ModeleId { get; set; }

        [Required(ErrorMessage = "La finition du véhicule est obligatoire")]
        public int FinitionId { get; set; }

        [Required(ErrorMessage = "Le nom de la finition est obligatoire")]
        [MaxLength(50, ErrorMessage = "Le nom de la finition ne peut pas dépasser 50 caractères.")]
        public string FinitionNom { get; set; } = string.Empty;
    }
}