using SabaRecord.Core.Models;

namespace SabaRecord.Core;

public interface IPlayerRepository
{
	Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName);

	Task<Guid> GetProviderIdByCodeAsync(string code);

	Task<PlayerInfo> GetPlayerInfobyPlayerCodeAsync(string code);

	Task<GameInfo> GetGamesByProviderIdAsync(Guid gameProviderId);

	Task<Guid> GetGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);
}