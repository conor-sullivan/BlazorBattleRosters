using BlazorBattleRosters.Data.Models;

namespace BlazorBattleRosters.Data.IServices
{
    public interface IRostersService
    {
        // Create
        Task<RosterModel> CreateRoster(RosterModel roster, string userId);
        Task<UnitModel> CreateOrUpdateUnit(UnitModel unit, string userId);

        // Read
        Task<RosterModel> GetRosterById(int rosterId);
        Task<List<RosterModel>> GetRostersByUserId(string userId);
        Task<List<UnitModel>> GetUnitsByRosterId(int rosterId);

        // Update
        Task<RosterModel> UpdateRoster(RosterModel roster);
        Task<UnitModel> UpdateUnit(UnitModel unit);

        // Delete
        Task<bool> DeleteRoster(int rosterId);
        Task<bool> DeleteUnit(int unitId);

	}
}
