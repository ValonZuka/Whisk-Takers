namespace WhiskTakers.Application.DTOs.Puntori
{
    public class PuntoriResponseDto
    {
        public int Id { get; set; }
        public string Emri { get; set; } = string.Empty;
        public string Mbiemri { get; set; } = string.Empty;
        public string Pozita { get; set; } = string.Empty;
        public int FabrikaId { get; set; }
        public string? FabrikaEmri { get; set; }
    }
}