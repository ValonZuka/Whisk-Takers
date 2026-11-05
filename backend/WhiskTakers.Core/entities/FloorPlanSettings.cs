namespace WhiskTakers.Core.Entities
{
    public class FloorPlanSettings
    {
        public int Id { get; set; }
        public string AreasJson { get; set; } = "[]";
        public string TablesJson { get; set; } = "[]";
    }
}
