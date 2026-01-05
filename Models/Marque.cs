using System.ComponentModel.DataAnnotations;

namespace Net_P5.Models
{
    public class Marque
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la marque est obligatoire.")]
        [MaxLength(50, ErrorMessage = "Le nom de la marque ne peut pas dépasser 50 caractères.")]
        [Display(Name = "Nom de la marque")]
        public string Nom { get; set; } = string.Empty;

        //Collections de navigation
        public virtual ICollection<Modele> Modeles { get; set; } = new List<Modele>();
    }
}
