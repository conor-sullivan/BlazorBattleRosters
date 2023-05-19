namespace BlazorBattleRosters.Data.Models
{
    public class WarGearModel
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Abilities { get; set; } = string.Empty;
        public int UnitId { get; set; }
    }
}
