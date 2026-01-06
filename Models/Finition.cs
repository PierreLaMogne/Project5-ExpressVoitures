using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.Models
{
    public class Finition
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la finition est obligatoire.")]
        [MaxLength(50, ErrorMessage = "Le nom de la finition ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Nom de la finition")]
        public string Nom { get; set; } = string.Empty;

        //Clé étrangère
        [ForeignKey(nameof(ModeleId))]
        public int ModeleId { get; set; }
        public virtual Modele Modele { get; set; } = null!;

        //Collections de navigation
        public virtual ICollection<Voiture> Voitures { get; set; } = new List<Voiture>();
    }
}
