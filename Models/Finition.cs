namespace Net_P5.Models
{
    public class Finition
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        //Clé étrangère
        public int ModeleId { get; set; }
        public virtual Modele Modele { get; set; }

        //Collections de navigation
        public virtual ICollection<Voiture> Voitures { get; set; } = new List<Voiture>();
    }
}
