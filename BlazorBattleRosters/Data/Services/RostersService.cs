using BlazorBattleRosters.Data.IServices;
using BlazorBattleRosters.Data.Models;
using BlazorBattleRosters.Shared;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BlazorBattleRosters.Data.Services
{
    public class RostersService : IRostersService
    {
        // Initialize the variables for the database connection string.
        private IConfiguration _configuration { get; }
        private string? _connectionString { get; }


        // Configure the RosterService
        /// <summary>
        ///     Constructor defines the database connection string.
        /// </summary>
        /// <param name="configuration">Configuration from appsettings.json</param>
        public RostersService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SecondDB");
        }


        // Create
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

        public async Task<UnitModel> CreateOrUpdateUnit(UnitModel unit, string userId)
        {
            // If UnitMode.Id is greater than 0, run update stored procedure.
            if (unit.Id <= 0)
            {
                return await CreateUnit(unit, userId);
            }
            else
            {
                return await UpdateUnit(unit);
            }
        }

        private async Task<UnitModel> CreateUnit(UnitModel unit, string userId)
        {
            try
            {
                // -- New Insert --
                // Insert UnitModel into db and return new UnitModel.Id
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@Name", unit.Name);
                    p.Add("@Movement", unit.Movement);
                    p.Add("@WeaponSkill", unit.WeaponSkill);
                    p.Add("@BallisticSkill", unit.BallisticSkill);
                    p.Add("@Strength", unit.Strength);
                    p.Add("@Toughness", unit.Toughness);
                    p.Add("@Wounds", unit.Wounds);
                    p.Add("@Attacks", unit.Attacks);
                    p.Add("@Leadership", unit.Leadership);
                    p.Add("@SaveRoll", unit.SaveRoll);
                    p.Add("@PointsValue", unit.PointsValue);
                    p.Add("@OwnerId", userId);
                    p.Add("@RosterId", unit.RosterId);
                    await connection.ExecuteAsync("dbo.sp_Units_Create", p, commandType: CommandType.StoredProcedure);
                    unit.Id = p.Get<int>("@id");

                    // Create keywords
                    if (unit.Keywords != null)
                    {
                        await CreateKeywords(unit.Keywords, unit.Id);
                    }

                    // Create WarGear
                    if (unit.WarGear != null)
                    {
                        await CreateWarGears(unit.WarGear, unit.Id);
                    }

                    // Create Weapons and WeaponAblities
                    if (unit.Weapons != null)
                    {
                        await CreateWeapons(unit.Weapons, unit.Id);
                    }

                    return unit;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<KeywordsModel>> CreateKeywords(List<KeywordsModel> keywords, int unitId)
        {
            try
            {
                List<KeywordsModel> savedKeywords = new List<KeywordsModel>();
                foreach (KeywordsModel keyword in keywords)
                {
                    savedKeywords.Add(await CreateKeyword(keyword, unitId));
                }
                return savedKeywords;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<KeywordsModel> CreateKeyword(KeywordsModel keyword, int unitId)
        {
            try
            {
                // -- New Insert --
                // Insert KeywordModel into db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@UnitId", unitId);
                    p.Add("@Name", keyword.Name);
                    await connection.ExecuteAsync("dbo.sp_Keywords_Create", p, commandType: CommandType.StoredProcedure);
                    keyword.Id = p.Get<int>("@id");
                    return keyword;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<WarGearModel>> CreateWarGears(List<WarGearModel> warGears, int unitId)
        {
            try
            {
                List<WarGearModel> savedWarGear = new List<WarGearModel>();
                foreach (WarGearModel warGear in warGears)
                {
                    savedWarGear.Add(await CreateWarGear(warGear, unitId));
                }
                return savedWarGear;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<WeaponModel>> CreateWeapons(List<WeaponModel> weapons, int unitId)
        {
            try
            {
                List<WeaponModel> savedWeapons = new List<WeaponModel>();
                foreach (WeaponModel weapon in weapons)
                {
                    savedWeapons.Add(await CreateWeapon(weapon, unitId));
                }
                return savedWeapons;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<WarGearModel> CreateWarGear(WarGearModel warGear, int unitId)
        {
            try
            {
                // -- New Insert --
                // Insert WarGearModel into db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@UnitId", unitId);
                    p.Add("@Name", warGear.Name);
                    p.Add("@Abilities", warGear.Abilities);
                    await connection.ExecuteAsync("dbo.sp_WarGear_Create", p, commandType: CommandType.StoredProcedure);
                    warGear.Id = p.Get<int>("@id");
                    return warGear;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<WeaponModel> CreateWeapon(WeaponModel weapon, int unitId)
        {
            try
            {
                // -- New Insert --
                // Insert WeaponModel into db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@UnitId", unitId);
                    p.Add("@Name", weapon.Name);
                    p.Add("@Type", weapon.Type);
                    p.Add("@Range", weapon.Range);
                    p.Add("@Strength", weapon.Strength);
                    p.Add("@ArmorPiercing", weapon.ArmorPiercing);
                    p.Add("@Damage", weapon.Damage);
                    await connection.ExecuteAsync("dbo.sp_Weapons_Create", p, commandType: CommandType.StoredProcedure);
                    weapon.Id = p.Get<int>("@id");
                    return weapon;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<WeaponAbilityModel> CreateWeaponAbility(WeaponAbilityModel weaponAbility, int weaponId)
        {
            try
            {
                // -- New Insert --
                // Insert WeaponAbilityModel into db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                    p.Add("@WeaponId", weaponId);
                    p.Add("@Name", weaponAbility.Name);
                    p.Add("@Rules", weaponAbility.Rules);
                    await connection.ExecuteAsync("dbo.sp_WeaponAbilities_Create", p, commandType: CommandType.StoredProcedure);
                    weaponAbility.Id = p.Get<int>("@id");
                    return weaponAbility;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Read
        /// <summary>
        ///     Execute stored procedure to get all rosters that belong to a given user.
        /// </summary>
        /// <param name="userId">User ID provided by Identity.</param>
        /// <returns>List list of RosterModel.</returns>
        public async Task<List<RosterModel>> GetRostersByUserId(string userId)
        {
            List<RosterModel> rosters;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@OwnerId", userId);
                var results = await connection.QueryAsync<RosterModel>("dbo.sp_Rosters_GetAllByUserId", p, commandType: CommandType.StoredProcedure);
                rosters = results.ToList();
            }
            return rosters;
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
        /// <returns>List List of UnitModel.</returns>
        public async Task<List<UnitModel>> GetUnitsByRosterId(int rosterId)
        {
            List<UnitModel> units;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@RosterId", rosterId);
                var results = await connection.QueryAsync<UnitModel>("dbo.sp_Units_GetByRosterId", p, commandType: CommandType.StoredProcedure);
                units = results.ToList();
            }
            foreach (UnitModel unit in units)
            {
                unit.Weapons = await GetWeaponsByUnitId(unit.Id);
                unit.Keywords = await GetUnitKeywordsByUnitId(unit.Id);
                unit.WarGear = await GetWarGearByUnitId(unit.Id);
            }
            return units;
        }

        private async Task<List<KeywordsModel>> GetUnitKeywordsByUnitId(int unitId)
        {
            List<KeywordsModel> unitKeywords;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                var results = await connection.QueryAsync<KeywordsModel>("dbo.sp_Keywords_GetByUnitId", p, commandType: CommandType.StoredProcedure);
                unitKeywords = results.ToList();
            }
            return unitKeywords;
        }

        /// <summary>
        ///     Execute stored procedure to get all weapons that belong to a given unit.
        /// </summary>
        /// <param name="unitId">UnitModel.Id</param>
        /// <returns>List list of WeaponModel.</returns>
        private async Task<List<WeaponModel>> GetWeaponsByUnitId(int unitId)
        {
            List<WeaponModel> weapons;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                var results = await connection.QueryAsync<WeaponModel>("dbo.sp_Weapons_GetByUnitId", p, commandType: CommandType.StoredProcedure);
                weapons = results.ToList();
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
        /// <returns>List list of WeaponAblitiesModel</returns>
        private async Task<List<WeaponAbilityModel>> GetWeaponAbilitiesByWeaponId(int weaponId)
        {
            List<WeaponAbilityModel> weaponAblities;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@WeaponId", weaponId);
                var results = await connection.QueryAsync<WeaponAbilityModel>("dbo.sp_WeaponAbilities_GetByWeaponId", p, commandType: CommandType.StoredProcedure);
                weaponAblities = results.ToList();
            }
            return weaponAblities;
        }

        /// <summary>
        ///     Execute stored procedure to get all wargear that belongs to a given unit.
        /// </summary>
        /// <param name="unitId">UnitModel.Id</param>
        /// <returns>List list of WarGearModel.</returns>
        private async Task<List<WarGearModel>> GetWarGearByUnitId(int unitId)
        {
            List<WarGearModel> warGear;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                var results = await connection.QueryAsync<WarGearModel>("dbo.sp_WarGear_GetByUnitId", p, commandType: CommandType.StoredProcedure);
                warGear = results.ToList();
            }
            return warGear;
        }

        /// <summary>
        ///     Execute stored procedure to get all unit abilities that belong to a given unit.
        /// </summary>
        /// <param name="unitId">UnitModel.Id</param>
        /// <returns>List list of UnitAbilityModel.</returns>
        private async Task<List<UnitAbilityModel>> GetUnitAbilitiesByUnitId(int unitId)
        {
            List<UnitAbilityModel> unitAbilities;
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UnitId", unitId);
                var results = await connection.QueryAsync<UnitAbilityModel>("dbo.sp_UnitAbilities_GetByUnitId", p, commandType: CommandType.StoredProcedure);
                unitAbilities = results.ToList();
            }
            return unitAbilities;
        }


        // Update
        public async Task<RosterModel> UpdateRoster(RosterModel roster)
        {
            try
            {
                // -- Update --
                // Update RosterModel in db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", roster.Id);
                    p.Add("@Name", roster.Name);
                    await connection.ExecuteAsync("dbo.sp_Rosters_Update", p, commandType: CommandType.StoredProcedure);
                    return roster;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UnitModel> UpdateUnit(UnitModel unit)
        {
            // -- Update --
            // Update UnitModel with UnitId
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", unit.Id);
                    p.Add("@Name", unit.Name);
                    p.Add("@Movement", unit.Movement);
                    p.Add("@WeaponSkill", unit.WeaponSkill);
                    p.Add("@BallisticSkill", unit.BallisticSkill);
                    p.Add("@Strength", unit.Strength);
                    p.Add("@Toughness", unit.Toughness);
                    p.Add("@Wounds", unit.Wounds);
                    p.Add("@Attacks", unit.Attacks);
                    p.Add("@Leadership", unit.Leadership);
                    p.Add("@SaveRoll", unit.SaveRoll);
                    p.Add("@PointsValue", unit.PointsValue);
                    await connection.ExecuteAsync("dbo.sp_Units_Update", p, commandType: CommandType.StoredProcedure);

                    // Update Keywords
                    if (unit.Keywords != null)
                    {
                        await UpdateKeywordsInUnit(unit.Keywords, unit.Id);
                    }

                    // Update WarGears
                    if (unit.WarGear != null)
                    {
                        await UpdateWarGearsInUnit(unit.WarGear, unit.Id);
                    }

                    // Update Weapons and Weapon Abilities
                    if (unit.Weapons != null)
                    {
                        await UpdateWeaponsInUnit(unit.Weapons, unit.Id);
                    }

                    return unit;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task UpdateKeywordsInUnit(List<KeywordsModel> keywords, int unitId)
        {
            try
            {
                // Delete keywords in db that have been removed in UI.
                List<KeywordsModel> keywordsToDelete = await GetKeywordsToDelete(keywords);
                foreach (KeywordsModel keyword in keywordsToDelete)
                {
                    await DeleteKeyword(keyword.Id);
                }

                // Update or Create keywords in db.
                foreach (KeywordsModel keyword in keywords)
                {
                    await CreateOrUpdateKeyword(keyword, unitId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<KeywordsModel> UpdateKeyword(KeywordsModel keyword)
        {
            try
            {
                // -- Update --
                // Update KeywordModel in db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", keyword.Id);
                    p.Add("@Name", keyword.Name);
                    await connection.ExecuteAsync("dbo.sp_Keywords_Update", p, commandType: CommandType.StoredProcedure);
                    return keyword;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateWeaponsInUnit(List<WeaponModel> weapons, int unitId)
        {
            try
            {
                // Delete weapons in db that have been removed in UI.
                List<WeaponModel> weaponsToDelete = await GetWeaponsToDelete(weapons);
                foreach (WeaponModel weapon in weaponsToDelete)
                {
                    await DeleteWeapon(weapon.Id);
                }

                // Update or Create warGear in db.
                foreach (WeaponModel weapon in weapons)
                {
                    await CreateOrUpdateWeapon(weapon, unitId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<WeaponModel> UpdateWeapon(WeaponModel weapon)
        {
            try
            {
                // -- Update --
                // Update Weapon in db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", weapon.Id);
                    p.Add("@Name", weapon.Name);
                    p.Add("@Type", weapon.Type);
                    p.Add("@Range", weapon.Range);
                    p.Add("@Strength", weapon.Strength);
                    p.Add("@ArmorPiercing", weapon.ArmorPiercing);
                    p.Add("@Damage", weapon.Damage);
                    await connection.ExecuteAsync("dbo.sp_Weapons_Update", p, commandType: CommandType.StoredProcedure);

                    // Update Weapon Abilities in db.
                    if (weapon.Abilities != null)
                    {
                        await UpdateWeaponAbilitiesInWeapon(weapon.Abilities, weapon.Id);
                    }

                    return weapon;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateWeaponAbilitiesInWeapon(List<WeaponAbilityModel> weaponAbilities, int weaponId)
        {
            try
            {
                // Delete Weapon Abilities in db that have been removed in UI.
                List<WeaponAbilityModel> weaponAbilitiesToDelete = await GetWeaponAbilitiesToDelete(weaponAbilities);
                foreach (WeaponAbilityModel ability in weaponAbilitiesToDelete)
                {
                    await DeleteWeaponAbility(ability.Id);
                }

                // Update or Create Weapon Ability in db.
                foreach (WeaponAbilityModel ability in weaponAbilities)
                {
                    await CreateOrUpdateWeaponAbility(ability, weaponId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<WeaponAbilityModel> UpdateWeaponAbility(WeaponAbilityModel weaponAbility)
        {
            try
            {
                // -- Update --
                // Update WeaponAbility in db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", weaponAbility.Id);
                    p.Add("@Name", weaponAbility.Name);
                    p.Add("@Rules", weaponAbility.Rules);
                    await connection.ExecuteAsync("dbo.sp_WeaponAbilities_Update", p, commandType: CommandType.StoredProcedure);

                    return weaponAbility;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateWarGearsInUnit(List<WarGearModel> warGears, int unitId)
        {
            try
            {
                // Delete warGears in db that have been removed in UI.
                List<WarGearModel> warGearsToDelete = await GetWarGearToDelete(warGears);
                foreach (WarGearModel warGear in warGearsToDelete)
                {
                    await DeleteWarGear(warGear.Id);
                }

                // Update or Create warGear in db.
                foreach (WarGearModel warGear in warGears)
                {
                    await CreateOrUpdateWarGear(warGear, unitId);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<WarGearModel> UpdateWarGear(WarGearModel warGear)
        {
            try
            {
                // -- Update --
                // Update WarGear in db
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", warGear.Id);
                    p.Add("@Name", warGear.Name);
                    p.Add("@Abilities", warGear.Abilities);
                    await connection.ExecuteAsync("dbo.sp_WarGear_Update", p, commandType: CommandType.StoredProcedure);

                    return warGear;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Delete
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

        public async Task<bool> DeleteUnit(int unitId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", unitId);
                    await connection.ExecuteAsync("dbo.sp_Units_Delete", p, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        private async Task<bool> DeleteKeyword(int keywordId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", keywordId);
                    await connection.ExecuteAsync("dbo.sp_Keywords_Delete", p, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        private async Task<bool> DeleteWeapon(int weaponId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", weaponId);
                    await connection.ExecuteAsync("dbo.sp_Weapons_Delete", p, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        private async Task<bool> DeleteWeaponAbility(int weaponAbilityId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", weaponAbilityId);
                    await connection.ExecuteAsync("dbo.sp_Weapons_Delete", p, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        private async Task<bool> DeleteWarGear(int warGearId)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_connectionString))
                {
                    var p = new DynamicParameters();
                    p.Add("@id", warGearId);
                    await connection.ExecuteAsync("dbo.sp_Wargear_Delete", p, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }


        // Helper Methods
        private async Task CreateOrUpdateKeyword(KeywordsModel keyword, int unitId)
        {
            // If Keyword.Id is greater than 0, run update stored procedure.
            if (keyword.Id <= 0)
            {
                await CreateKeyword(keyword, unitId);
            }
            else
            {
                await UpdateKeyword(keyword);
            }
        }

        private async Task CreateOrUpdateWeapon(WeaponModel weapon, int unitId)
        {
            // If Weapon.Id is greater than 0, run update stored procedure.
            if (weapon.Id <= 0)
            {
                await CreateWeapon(weapon, unitId);
            }
            else
            {
                await UpdateWeapon(weapon);
            }
        }

        private async Task CreateOrUpdateWeaponAbility(WeaponAbilityModel ability, int weaponId)
        {
            // If WeaponAbility.Id is greater than 0, run update stored procedure.
            if (ability.Id <= 0)
            {
                await CreateWeaponAbility(ability, weaponId);
            }
            else
            {
                await UpdateWeaponAbility(ability);
            }
        }

        private async Task CreateOrUpdateWarGear(WarGearModel warGear, int unitId)
        {
            // If WarGear.Id is greater than 0, run update stored procedure.
            if (warGear.Id <= 0)
            {
                await CreateWarGear(warGear, unitId);
            }
            else
            {
                await UpdateWarGear(warGear);
            }
        }

        private async Task<List<KeywordsModel>> GetKeywordsToDelete(List<KeywordsModel> keywords)
        {
            List<KeywordsModel> keywordsInDb = await GetUnitKeywordsByUnitId(keywords.First().UnitId);
            List<KeywordsModel> keywordsToDelete = keywordsInDb.Except(keywords).ToList();
            return keywordsToDelete;
        }

        private async Task<List<WeaponModel>> GetWeaponsToDelete(List<WeaponModel> weapons)
        {
            List<WeaponModel> weaponsInDb = await GetWeaponsByUnitId(weapons.First().UnitId);
            List<WeaponModel> weaponsToDelete = weaponsInDb.Except(weapons).ToList();
            return weaponsToDelete;
        }

        private async Task<List<WeaponAbilityModel>> GetWeaponAbilitiesToDelete(List<WeaponAbilityModel> weaponAbilities)
        {
            List<WeaponAbilityModel> weaponAbilitiesInDb = await GetWeaponAbilitiesByWeaponId(weaponAbilities.First().WeaponId);
            List<WeaponAbilityModel> weaponAbilitiestoDelete = weaponAbilitiesInDb.Except(weaponAbilities).ToList();
            return weaponAbilitiestoDelete;
        }

        private async Task<List<WarGearModel>> GetWarGearToDelete(List<WarGearModel> wargear)
        {
            List<WarGearModel> warGearInDb = await GetWarGearByUnitId(wargear.First().UnitId);
            List<WarGearModel> warGearToDelete = warGearInDb.Except(wargear).ToList();
            return warGearToDelete;
        }
    }
}
