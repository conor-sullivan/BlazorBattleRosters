using BlazorBattleRosters.Data.IServices;
using BlazorBattleRosters.Data.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BlazorBattleRosters.Data.Services
{
    public class RostersService : IRostersService
    {
        // Initialize the variables for the database connection string.
        public IConfiguration _configuration { get; }
        public string? _connectionString { get; }

        /// <summary>
        ///     Constructor defines the database connection string.
        /// </summary>
        /// <param name="configuration">Configuration from appsettings.json</param>
        public RostersService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SecondDB");
        }

        /// <summary>
        ///     Execute stored procedure to delete roster from database.
        /// </summary>
        /// <param name="rosterId">RosterModel.Id</param>
        /// <returns>True or throws exception.</returns>
        public async Task<bool> DeleteRoster(int rosterId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", rosterId);
                    await connection.ExecuteAsync("dbo.sp_Rosters_Delete", p, commandType: CommandType.StoredProcedure);
                }
               
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        /// <summary>
        ///     Execute stored procedure to get a single roster from the database.
        ///     Execute method GetUnitsByRosterId() to populate roster.Units.
        /// </summary>
        /// <param name="rosterId">RosterModel.Id</param>
        /// <returns>A single roster model.</returns>
        public async Task<RosterModel> GetRosterById(int rosterId)
        {
            RosterModel roster = new RosterModel();
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@id", rosterId);
                roster = await connection.QueryFirstOrDefaultAsync<RosterModel>("dbo.sp_Rosters_GetRosterById", p, commandType: CommandType.StoredProcedure);
            }
            roster.Units = await GetUnitsByRosterId(rosterId);
            return roster;
        }

        /// <summary>
        ///     Execute stored procedure to get all units that belong to a given roster.
        /// </summary>
        /// <param name="rosterId">RosterModel.Id</param>
        /// <returns>IEnumerable List of UnitModel.</returns>
        public async Task<IEnumerable<UnitModel>> GetUnitsByRosterId(int rosterId)
        {
            IEnumerable<UnitModel> units;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@RosterId", rosterId);
                units = await connection.QueryAsync<UnitModel>("dbo.sp_Units_GetByRosterId", p, commandType: CommandType.StoredProcedure);
            }
            foreach (UnitModel unit in units)
            {
                unit.Weapons = await GetWeaponsByUnitId(unit.Id);
            }
            return units;
        }

        /// <summary>
        ///     Execute stored procedure to get all rosters that belong to a given user.
        /// </summary>
        /// <param name="userId">User ID provided by Identity.</param>
        /// <returns>IEnumerable list of RosterModel.</returns>
        public async Task<IEnumerable<RosterModel>> GetRostersByUserId(string userId)
        {
            IEnumerable<RosterModel> rosters;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@OwnerId", userId);
                rosters = await connection.QueryAsync<RosterModel>("dbo.sp_Rosters_GetAllByUserId", p, commandType: CommandType.StoredProcedure);
            }
            return rosters;
        }

        /// <summary>
        ///     Execute stored procedure to get all weapons that belong to a given unit.
        /// </summary>
        /// <param name="unitId">UnitModel.Id</param>
        /// <returns>IEnumerable list of WeaponModel.</returns>
        public async Task<IEnumerable<WeaponModel>> GetWeaponsByUnitId(int unitId)
        {
            IEnumerable<WeaponModel> weapons;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                weapons = await connection.QueryAsync<WeaponModel>("dbo.sp_Weapons_GetByUnitId", p, commandType: CommandType.StoredProcedure);
            }

            foreach (WeaponModel weapon in weapons)
            {
                weapon.Abilities = await GetWeaponAbilitiesByWeaponId(weapon.Id);
            }
            return weapons;
        }

        /// <summary>
        ///     Execute stored procedure to get all weapon abilities that belong to a given weapon.
        /// </summary>
        /// <param name="weaponId">WeaponModel.Id</param>
        /// <returns>IEnumerable list of WeaponAblitiesModel</returns>
        public async Task<IEnumerable<WeaponAbilityModel>> GetWeaponAbilitiesByWeaponId(int weaponId)
        {
            IEnumerable<WeaponAbilityModel> weaponAblities;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@WeaponId", weaponId);
                weaponAblities = await connection.QueryAsync<WeaponAbilityModel>("dbo.sp_WeaponAbilities_GetByWeaponId", p, commandType: CommandType.StoredProcedure);
            }
            return weaponAblities;
        }

        /// <summary>
        ///     Execute stored procedure to save a new roster to the database.
        /// </summary>
        /// <param name="roster">RosterModel</param>
        /// <param name="userId">User Id provided by Identity.</param>
        /// <returns>Created RosterModel, including the new roster ID or throws exception.</returns>
        public async Task<RosterModel> CreateRoster(RosterModel roster, string userId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@OwnerId", userId);
                    p.Add("@Name", roster.Name);
                    await connection.ExecuteAsync("dbo.sp_Rosters_Create", p, commandType: CommandType.StoredProcedure);
                    roster.Id = p.Get<int>("@id");
                }
            }
            
            catch (Exception)
            {

                throw;
            }
            return roster;
        }

        /// <summary>
        ///     Execute stored procedure to get all wargear that belongs to a given unit.
        /// </summary>
        /// <param name="unitId">UnitModel.Id</param>
        /// <returns>IEnumerable list of WarGearModel.</returns>
        public async Task<IEnumerable<WarGearModel>> GetWarGearByUnitId(int unitId)
        {
            IEnumerable<WarGearModel> warGear;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                warGear = await connection.QueryAsync<WarGearModel>("dbo.sp_WarGear_GetByUnitId", p, commandType: CommandType.StoredProcedure);
            }
            return warGear;
        }

        /// <summary>
        ///     Execute stored procedure to get all unit abilities that belong to a given unit.
        /// </summary>
        /// <param name="unitId">UnitModel.Id</param>
        /// <returns>IEnumerable list of UnitAbilityModel.</returns>
        public async Task<IEnumerable<UnitAbilityModel>> GetUnitAbilitiesByUnitId(int unitId)
        {
            IEnumerable<UnitAbilityModel> unitAbilities;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                unitAbilities = await connection.QueryAsync<UnitAbilityModel>("dbo.sp_UnitAbilities_GetByUnitId", p, commandType: CommandType.StoredProcedure);
            }
            return unitAbilities;
        }
    }
}
