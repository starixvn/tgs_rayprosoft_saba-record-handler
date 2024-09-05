using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaRecord.Core;
using SabaRecord.Core.Models;

namespace SabaRecord.Repositories
{
	public class PlayerRepositoryImp : IPlayerRepository
	{
		private readonly string _connectionString;

		public PlayerRepositoryImp(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Tgs")!;
		}

		public async Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				foreach (var property in typeof(TCriteria).GetProperties())
				{
					var value = property.GetValue(criteria);
					parameters.Add(property.Name.ToLower(), value);
				}

				string sqlQuery = $"CALL {spName}({string.Join(", ", typeof(TCriteria).GetProperties().Select(p => $"@{p.Name.ToLower()}"))})";

				var result = await db.QueryFirstAsync<TResult>(sqlQuery, parameters);

				return result;
			}
		}

		public async Task<Guid> GetProviderIdByCodeAsync(string code)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = "SELECT ga.Id FROM dbo.game_providers ga WHERE ga.code = @Code";
				var result = await db.QueryFirstOrDefaultAsync<Guid>(sqlQuery, new { Code = code });
				return result;
			}
		}

		public async Task<PlayerInfo> GetPlayerInfobyPlayerCodeAsync(string code)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sql = @"
							SELECT
								r.Id AS ResellerId,
								r.Code AS ResellerCode,
								l.Id AS LicenseeId,
								l.Code AS LicenseeCode,
								b.Id AS BrandId,
								b.Code AS BrandCode,
								p.player_code AS PlayerCode,
								p.username AS Username,
								p.default_wallet_id AS WalletId,
								p.default_wallet_currency_code AS CurrencyCode
							FROM
								dbo.Players p
							INNER JOIN
								dbo.Brands b ON p.brand_id = b.id
							INNER JOIN
								dbo.Licensees l ON b.licensee_id = l.id
							INNER JOIN
								dbo.Resellers r ON l.reseller_id = r.id
							WHERE
								p.player_code = @PlayerCode;";

				var parameters = new { PlayerCode = code };
				var playerInfo = await db.QueryFirstOrDefaultAsync<PlayerInfo>(sql, parameters); // check correct property later
				return playerInfo;
			}
		}

		public async Task<GameInfo> GetGamesByProviderIdAsync(Guid gameProviderId)
		{
			gameProviderId = gameProviderId == default ? Guid.Parse("9d57276a-15ee-4e3f-a1cb-6cb7dd7bb641") : gameProviderId;
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = @"
            SELECT
                id as GameId,
                status,
                external_id AS ExternalGameId,
                game_provider_id AS GameProviderId,
                sub_game AS SubGame,
                available_currencies AS AvailableCurrencies,
                code as GameCode,
                name,
                sub_game_provider_code AS SubGameProviderCode
            FROM
                dbo.games
            WHERE
                game_provider_id = @GameProviderId";

				var parameters = new { GameProviderId = gameProviderId };
				var games = await db.QueryFirstOrDefaultAsync<GameInfo>(sqlQuery, parameters); // check correct property later
				return games;
			}
		}

		public async Task<Guid> GetGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = @"
									SELECT
										bgp.game_provider_configuration_id
									FROM
										dbo.brand_game_providers AS bgp
									WHERE
										bgp.game_provider_id = @GameProviderId
										AND bgp.brand_id = @BrandId";

				var parameters = new { GameProviderId = gameProviderId, BrandId = brandId };
				var result = await db.QueryFirstOrDefaultAsync<Guid>(sqlQuery, parameters);
				return result;
			}
		}
	}
}