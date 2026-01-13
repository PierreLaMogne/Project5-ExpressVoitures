using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.Models
{
    public class Reparation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le détail de la réparation est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le détail de la réparation ne peut pas dépasser 200 caractères.")]
        [Display(Name = "Détail des réparations")]
        public string Detail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le coût de la réparation est obligatoire.")]
        [Precision(12, 2)]
        [Range(0, (double)decimal.MaxValue)]
        [Display(Name = "Coût de la réparation")]
        public decimal Cout { get; set; }

        [Required(ErrorMessage = "La date de disponibilité est obligatoire.")]
        [Display(Name = "Date de disponibilité")]
        public DateOnly DateDisponibilite { get; set; }

        //Clé étrangère

        [ForeignKey(nameof(VoitureId))]
        public int VoitureId { get; set; }
        public virtual Voiture Voiture { get; set; } = null!;
    }
}
