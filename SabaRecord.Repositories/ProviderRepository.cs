using System.Data;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using SabaRecord.Core.Configuarations;
using SabaRecord.Core.Models;
using SabaRecord.Core.Repositories;

namespace SabaRecord.Repositories
{
	public class ProviderRepository : IProviderRepository
	{
		private readonly string connectionString;
		private readonly IMemoryCache memoryCache;
		private readonly CacheSettingsOptions cacheSettings;

		public ProviderRepository(IConfiguration configuration, IMemoryCache memoryCache, IOptions<CacheSettingsOptions> cacheSettings)
		{
			connectionString = configuration.GetConnectionString("Tgs")!;
			this.memoryCache = memoryCache;
			this.cacheSettings = cacheSettings.Value;
		}

		public async Task<Guid?> GetProviderIdByCodeAsync(string code)
		{
			string key = string.Concat(typeof(ProviderRepository).Namespace!.Replace(".", "-").ToLower(), ":Provider:", code);
			if (!memoryCache.TryGetValue(key, out Guid? id))
			{
				using (IDbConnection db = new NpgsqlConnection(connectionString))
				{
					string sqlQuery = "SELECT ga.Id FROM dbo.game_providers ga WHERE ga.code = @Code";
					id = await db.QueryFirstOrDefaultAsync<Guid>(sqlQuery, new { Code = code });
				}

				if (id is not null)
				{
					return memoryCache.Set(key, id, TimeSpan.FromMinutes(cacheSettings.ExpiredTimeInMinute));
				}
			}

			return id;
		}

		public async Task<Guid?> GetGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId)
		{
			string key = $"{typeof(ProviderRepository).Namespace!.Replace(".", "-").ToLower()}:GameProviderConfiguration:{gameProviderId}";
			if (!memoryCache.TryGetValue(key, out Guid? id))
				using (IDbConnection db = new NpgsqlConnection(connectionString))
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
					id = await db.QueryFirstOrDefaultAsync<Guid>(sqlQuery, parameters);

					if (id is not null)
					{
						return memoryCache.Set(key, id, TimeSpan.FromMinutes(cacheSettings.ExpiredTimeInMinute));
					}
				}

			return id;
		}

		public async Task<GameInfo?> GetGameInfoByExternalIdAsync(Guid gameProviderId, string externalId)
		{
			var key = BuildCacheKey($"GetGameInfoByExternalId::{gameProviderId}:{externalId}");

			if (!memoryCache.TryGetValue(key, out GameInfo? gameInfo))
			{
				using IDbConnection db = new NpgsqlConnection(connectionString);
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
							external_id = @ExternalId AND game_provider_id = @GameProviderId";

				var parameters = new { ExternalId = externalId, GameProviderId = gameProviderId };

				gameInfo = await db.QueryFirstOrDefaultAsync<GameInfo>(sqlQuery, parameters);

				if (gameInfo is not null)
				{
					return memoryCache.Set(key, gameInfo, TimeSpan.FromMinutes(cacheSettings.ExpiredTimeInMinute));
				}
			}

			return gameInfo is null ? await GetGameInfoByExternalIdAsync(gameProviderId, "0") : gameInfo;
		}

		private static string BuildCacheKey(string action)
			=> $"{typeof(ProviderRepository).Namespace!.Replace(".", "-").ToLower()}:{action}";
	}
}