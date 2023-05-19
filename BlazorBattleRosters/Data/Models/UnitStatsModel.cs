namespace BlazorBattleRosters.Data.Models
{
    public class UnitStatsModel
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public int UnitId { get; set; }
    }
}
