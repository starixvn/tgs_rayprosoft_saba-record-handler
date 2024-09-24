using SabaRecord.Core.Models;

namespace SabaRecord.Core.Repositories;

public interface IProviderRepository
{
	Task<Guid?> GetProviderIdByCodeAsync(string code);

	Task<Guid?> GetGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);

	Task<int?> GetGameIdByExternalIdAsync(Guid gameProviderId, string externalId);
}