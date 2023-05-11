namespace BlazorBattleRosters.Data.Models
{
    public class WeaponModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Range { get; set; }
        public int Strength { get; set; }
        public int ArmorPiercing { get; set; }
        public int Damage { get; set; }
        public int UnitId { get; set; }
        public IEnumerable<WeaponAbilityModel>? Abilities { get; set; }
    }
}
