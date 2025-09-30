using WhiskTakers.Application.DTOs.Puntori;

namespace WhiskTakers.Application.DTOs.Fabrika
{
    public class FabrikaResponseDto
    {
        public int Id { get; set; }
        public string EmriFabrikes { get; set; } = string.Empty;
        public string AdresaFabrikes { get; set; } = string.Empty;
        public List<PuntoriResponseDto> Puntori { get; set; } = new();
    }
}