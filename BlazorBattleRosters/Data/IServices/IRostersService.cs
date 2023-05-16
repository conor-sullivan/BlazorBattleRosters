using BlazorBattleRosters.Data.Models;

namespace BlazorBattleRosters.Data.IServices
{
    public interface IRostersService
    {
        Task<List<RosterModel>> GetRostersByUserId(string userId);
        Task<RosterModel> CreateRoster(RosterModel roster, string userId);
        Task<RosterModel> GetRosterById(int rosterId);
        Task<bool> DeleteRoster(int rosterId);
        Task<List<UnitModel>> GetUnitsByRosterId(int rosterId);
        Task<List<WeaponAbilityModel>> GetWeaponAbilitiesByWeaponId(int weaponId);
        Task<List<WeaponModel>> GetWeaponsByUnitId(int unitId);
        Task<List<WarGearModel>> GetWarGearByUnitId(int unitId);
        Task<List<UnitAbilityModel>> GetUnitAbilitiesByUnitId(int unitId);
        Task<List<KeywordsModel>> GetUnitKeywordsByUnitId(int unitId);


	}
}
