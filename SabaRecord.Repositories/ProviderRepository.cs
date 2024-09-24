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
				    id = await db.QueryFirstOrDefaultAsync<Guid>("SELECT dbo.game_provider_get_provider_id_by_code(@provider_code);", new { provider_code = code });
				}

				if (id is not null)
				{
					return memoryCache.Set(key, id, TimeSpan.FromHours(cacheSettings.ExpiredTimeInHour));
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
					var parameters = new { game_provider_id = gameProviderId, brand_id = brandId };

					id = await db.QueryFirstOrDefaultAsync<Guid>(" SELECT dbo.brand_game_provider_get_configuration_id(@game_provider_id, @brand_id);", parameters);

					if (id is not null)
					{
						return memoryCache.Set(key, id, TimeSpan.FromHours(cacheSettings.ExpiredTimeInHour));
					}
				}

			return id;
		}

		public async Task<int?> GetGameIdByExternalIdAsync(Guid gameProviderId, string externalId)
		{
			var key = BuildCacheKey($"GetGameIdByExternalId::{gameProviderId}:{externalId}");

			if (!memoryCache.TryGetValue(key, out int? gameId))
			{
				using IDbConnection db = new NpgsqlConnection(connectionString);

				var parameters = new { external_id = externalId, game_provider_id = gameProviderId };


			    gameId = await db.QueryFirstOrDefaultAsync<int>("SELECT dbo.game_get_game_id_by_external_id_and_provider_id(@external_id, @game_provider_id)", parameters);

				if (gameId is not null)
				{
					return memoryCache.Set(key, gameId, TimeSpan.FromMinutes(cacheSettings.ExpiredTimeInMinute));
				}
			}

			return gameId is null ? await GetGameIdByExternalIdAsync(gameProviderId, "0") : gameId;
		}

		private static string BuildCacheKey(string action)
			=> $"{typeof(ProviderRepository).Namespace!.Replace(".", "-").ToLower()}:{action}";
	}
}