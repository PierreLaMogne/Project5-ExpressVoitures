using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.Models
{
    public class Vente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La date de vente est obligatoire.")]
        [Display(Name = "Date de vente")]
        public DateOnly DateVente { get; set; }

        //Clé étrangère

        [ForeignKey(nameof(VoitureId))]
        public int VoitureId { get; set; }
        public virtual Voiture Voiture { get; set; } = null!;
    }
}
