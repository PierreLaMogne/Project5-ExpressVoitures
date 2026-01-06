using Microsoft.EntityFrameworkCore;
using Net_P5.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.Models
{
    public class Voiture
    {
        [Key]
        [Required(ErrorMessage = "Le code VIN est obligatoire.")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "Le code VIN doit contenir exactement 17 caractères alphanumériques (sans les lettres I, O, Q).")]
        [MaxLength(17)]
        [Display(Name = "Code VIN")]
        public required string CodeVIN { get; set; }

        [Required(ErrorMessage = "L'année de fabrication est obligatoire.")]
        [Display(Name = "Année de fabrication")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "L'année doit être au format AAAA.")]
        [CurrentYear(1990, ErrorMessage = "L'année doit être comprise entre 1990 et l'année en cours.")]
        public int Annee { get; set; }

        [Required(ErrorMessage = "La date d'achat est obligatoire.")]
        [Display(Name = "Date d'achat")]
        public DateOnly DateAchat { get; set; }

        [Required(ErrorMessage = "Le prix est obligatoire.")]
        [Display(Name = "Prix d'achat")]
        [Precision(12, 2)]
        public decimal PrixAchat { get; set; }

        [Required(ErrorMessage = "Le prix de vente est obligatoire.")]
        [Display(Name = "Prix de vente")]
        [Precision(12, 2)]
        public decimal PrixVente { get; set; }

        [NotMapped]
        [Display(Name = "Photo de la voiture")]
        public IFormFile? Photo { get; set; }

        [MaxLength(255)]
        [Display(Name = "Chemin de la photo")]
        public string? PhotoUrl { get; set; }

        [Display(Name = "Est-ce que la voiture est en vente ?")]
        public bool EnVente { get; set; } = false;

        //Propriétés calculées

        [NotMapped]
        public bool EstVendue => Ventes.Any();

        [NotMapped]
        public bool EstEnReparation => Reparations.Any(r => r.DateDisponibilite > DateOnly.FromDateTime(DateTime.Now));

        [NotMapped]
        public string Statut => EstVendue ? "Vendue" : EstEnReparation ? "En réparation" : EnVente ? "Disponible" : "Indisponible";

         [NotMapped]
        public string NomComplet => $"{Finition?.Modele?.Marque?.Nom} {Finition?.Modele?.Nom} {Finition?.Nom}";

        //Prix de vente calculé automatiquement
        //[NotMapped]
        //public decimal PrixVente => Reparations.Sum(r => r.Cout) + PrixAchat;


        //Clés étrangères

        [ForeignKey(nameof(MarqueId))]
        public int MarqueId { get; set; }
        public virtual Marque Marque { get; set; } = null!;

        [ForeignKey(nameof(ModeleId))]
        public int ModeleId { get; set; }
        public virtual Modele Modele { get; set; } = null!;

        [ForeignKey(nameof(FinitionId))]
        public int FinitionId { get; set; }
        public virtual Finition Finition { get; set; } = null!;

        //Collections de navigation

        public virtual ICollection<Reparation> Reparations { get; set; } = new List<Reparation>();
        public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
    }
}
