namespace WhiskTakers.Core.Entities
{
    public class Fabrika
    {
        public int Id { get; set; }
        public string EmriFabrikes { get; set; } = string.Empty;
        public string AdresaFabrikes { get; set; } = string.Empty;
        public ICollection<Puntori> Puntori { get; set; } = new List<Puntori>();
    }
}
