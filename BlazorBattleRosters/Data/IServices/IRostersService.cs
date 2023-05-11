using BlazorBattleRosters.Data.Models;

namespace BlazorBattleRosters.Data.IServices
{
    public interface IRostersService
    {
        Task<IEnumerable<RosterModel>> GetRosters(string userId);
        Task<bool> CreateRoster(RosterModel roster, string userId);
        Task<RosterModel> GetRosterById(int rosterId);
        Task<bool> DeleteRoster(int rosterId);
        Task<IEnumerable<UnitModel>> GetUnitsByRosterId(int rosterId);
    }
}
