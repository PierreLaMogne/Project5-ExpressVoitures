namespace Net_P5.Models
{
    public class Vente
    {
        public int Id { get; set; }
        public DateOnly DateVente { get; set; }

        //Clé étrangère
        public string VoitureCodeVIN { get; set; }
        public virtual Voiture Voiture { get; set; }
    }
}
