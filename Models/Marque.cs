namespace Net_P5.Models
{
    public class Marque
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        //Collections de navigation
        public virtual ICollection<Modele> Modeles { get; set; } = new List<Modele>();
    }
}
