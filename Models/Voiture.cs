using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Net_P5.Models
{
    public class Voiture
    {
        public string CodeVIN { get; set; }

        public int Annee { get; set; }

        public DateOnly DateAchat { get; set; }

        public decimal PrixAchat { get; set; }

        public bool EnVente { get; set; }

        public bool EstVendue => Ventes.Any();

        //Propriétés calculées

        public bool EstEnReparation => Reparations.Any(r => r.DateDisponibilite > DateOnly.FromDateTime(DateTime.Now);

        public string Statut => EstVendue ? "Vendue" : EstEnReparation ? "En réparation" : EnVente ? "Disponible" : "Indisponible";

        public decimal PrixVente => Reparations.Sum(r => r.Cout) + PrixAchat;

        public string NomComplet => $"{Finition?.Modele?.Marque?.Nom} {Finition?.Modele?.Nom} {Finition?.Nom}";

        //Clés étrangères

        public int FinitionId { get; set; }
        public virtual Finition Finition { get; set; }

        //Collections de navigation

        public virtual ICollection<Reparation> Reparations { get; set; } = new List<Reparation>();
        public virtual ICollection<Vente> Ventes { get; set; } = new List<Vente>();
    }
}
