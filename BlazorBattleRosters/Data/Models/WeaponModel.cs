namespace BlazorBattleRosters.Data.Models
{
    public class WeaponModel
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Range { get; set; } = 0;
        public int Strength { get; set; } = 0;
        public int ArmorPiercing { get; set; } = 0;
        public int Damage { get; set; } = 0;
        public int UnitId { get; set; } = 0;
        public List<WeaponAbilityModel>? Abilities { get; set; } = new List<WeaponAbilityModel>();
    }
}
