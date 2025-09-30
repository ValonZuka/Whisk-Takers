namespace WhiskTakers.Core.Entities
{
    public class Puntori
    {
        public int Id { get; set; }
            public string Emri { get; set; } = string.Empty;
            public string Mbiemri { get; set; } = string.Empty;

            public string Pozita { get; set; } = string.Empty;
        public Fabrika? Fabrika { get; set; }
        public int FabrikaId { get; set; }
    }
}
