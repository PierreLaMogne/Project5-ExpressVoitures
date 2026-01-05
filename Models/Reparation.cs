namespace Net_P5.Models
{
    public class Reparation
    {
        public int Id { get; set; }
        public string Detail { get; set; }
        public decimal Cout { get; set; }
        public DateOnly DateDisponibilite { get; set; }

        //Clé étrangère
        public string VoitureCodeVIN { get; set; }
        public virtual Voiture Voiture { get; set; }
    }
}
