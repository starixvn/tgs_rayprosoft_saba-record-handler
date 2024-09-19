namespace SabaRecord.Core.Repositories;

public interface IApiLogsRepository
{
	ValueTask<bool> AddAsync(string gameProviderCode, string action, string reqJson, string resJson, CancellationToken cancellationToken = default);
}