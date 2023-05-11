using BlazorBattleRosters.Data.Models;

namespace BlazorBattleRosters.Data.IServices
{
    public interface IRostersService
    {
        Task<IEnumerable<RosterModel>> GetRostersByUserId(string userId);
        Task<RosterModel> CreateRoster(RosterModel roster, string userId);
        Task<RosterModel> GetRosterById(int rosterId);
        Task<bool> DeleteRoster(int rosterId);
        Task<IEnumerable<UnitModel>> GetUnitsByRosterId(int rosterId);
        Task<IEnumerable<WeaponAbilityModel>> GetWeaponAbilitiesByWeaponId(int weaponId);
        Task<IEnumerable<WeaponModel>> GetWeaponsByUnitId(int unitId);
        Task<IEnumerable<WarGearModel>> GetWarGearByUnitId(int unitId);
        Task<IEnumerable<UnitAbilityModel>> GetUnitAbilitiesByUnitId(int unitId);

    }
}
