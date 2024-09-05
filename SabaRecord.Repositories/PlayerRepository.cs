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
					playerInfo = await db.QueryFirstOrDefaultAsync<PlayerInfo>(sql, parameters); // check correct property later
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