﻿namespace BlazorBattleRosters.Data.Models
{
    public class WeaponAbilityModel
    {
        public int Id { get; set; } = 0;
        public int WeaponId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Rules { get; set; } = string.Empty;
    }
}