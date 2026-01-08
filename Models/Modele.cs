using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.Models
{
    public class Modele
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du modèle est obligatoire.")]
        [MaxLength(50, ErrorMessage = "Le nom du modèle ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Nom du modèle")]
        public string Nom { get; set; } = string.Empty;

        //Propriétés calculées
        [NotMapped]
        public string NomComplet => $"{Marque.Nom} {Nom}";

        //Clé étrangère

        [ForeignKey(nameof(MarqueId))]
        public int MarqueId { get; set; }
        public virtual Marque Marque { get; set; } = null!;

        //Collections de navigation
        public virtual ICollection<Finition> Finitions { get; set; } = new List<Finition>();
    }
}
