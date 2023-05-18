using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorBattleRosters.Data.Models
{
    public class UnitModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public int Movement { get; set; }
        public int WeaponSkill { get; set; }
        public int BallisticSkill { get; set; }
        public int Strength { get; set; }
        public int Toughness { get; set; }
        public int Wounds { get; set; }
        public int Attacks { get; set; }
        public int Leadership { get; set; }
        public int SaveRoll { get; set; }
        public int NumberOfModels { get; set; }
        public int PowerLevel { get; set; }
        public int PointsValue { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public int RosterId { get; set; }
        public UnitImageModel? Image { get; set; } = new UnitImageModel();
        public List<KeywordsModel>? Keywords { get; set; } = new List<KeywordsModel>();
        public List<WarGearModel>? WarGear { get; set; } = new List<WarGearModel>();
        public List<WeaponModel>? Weapons { get; set; } = new List<WeaponModel>();
        public List<UnitAbilityModel>? Abilities { get; set; }
    }
}