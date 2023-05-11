namespace BlazorBattleRosters.Data.Models
{
    public class UnitImageModel
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int UnitId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
