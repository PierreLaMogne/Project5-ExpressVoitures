namespace Net_P5.Models
{
    public class Modele
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        
        //Clé étrangère
        public int MarqueId { get; set; }
        public virtual Marque Marque { get; set; }

        //Collections de navigation
        public virtual ICollection<Finition> Finitions { get; set; } = new List<Finition>();
    }
}
