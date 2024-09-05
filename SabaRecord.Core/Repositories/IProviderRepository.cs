using SabaRecord.Core.Models;

namespace SabaRecord.Core.Repositories;

public interface IProviderRepository
{
	Task<Guid?> GetProviderIdByCodeAsync(string code);

	Task<GameInfo?> GetGamesByProviderIdAsync(Guid gameProviderId);

	Task<Guid?> GetGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);
}