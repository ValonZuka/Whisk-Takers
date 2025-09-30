namespace WhiskTakers.Application.DTOs.Puntori
{
    public class PuntoriCreateDto
    {
        public string Emri { get; set; } = string.Empty;
        public string Mbiemri { get; set; } = string.Empty;
        public string Pozita { get; set; } = string.Empty;
        public int FabrikaId { get; set; }
    }
}   