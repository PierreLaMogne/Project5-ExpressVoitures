using Microsoft.EntityFrameworkCore;
using Net_P5.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.ViewModels
{
    public class MarqueViewModel
    {
        public int MarqueId { get; set; }

        [MaxLength(50, ErrorMessage = "Le nom de la marque ne peut pas dépasser 50 caractères.")]
        public string MarqueNom { get; set; } = string.Empty;
    }
}