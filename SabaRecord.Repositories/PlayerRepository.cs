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
	public class PlayerRepositoryImp : IPlayerRepository
	{
		private readonly string connectionString;
		private readonly IMemoryCache memoryCache;
		private readonly CacheSettingsOptions cacheSettings;

		public PlayerRepositoryImp(IConfiguration configuration, IMemoryCache memoryCache, IOptions<CacheSettingsOptions> cacheSettings)
		{
			connectionString = configuration.GetConnectionString("Tgs")!;
			this.memoryCache = memoryCache;
			this.cacheSettings = cacheSettings.Value;
		}

		public async Task<PlayerInfo?> GetPlayerInfobyPlayerCodeAsync(string code)
		{
			string key = $"{typeof(PlayerRepositoryImp).Namespace!.Replace(".", "-").ToLower()}:PlayerInfo:{code}";
			if (!memoryCache.TryGetValue(key, out PlayerInfo? playerInfo))
			{
				using (IDbConnection db = new NpgsqlConnection(connectionString))
				{
					var parameters = new { player_code = code };
				    playerInfo = await db.QueryFirstOrDefaultAsync<PlayerInfo>("SELECT * FROM dbo.player_get_player_info_by_player_code(@player_code);", parameters);
				}

				if (playerInfo is not null)
				{
					return memoryCache.Set(key, playerInfo, TimeSpan.FromMinutes(cacheSettings.ExpiredTimeInMinute));
				}
			}

			return playerInfo;
		}
	}
}