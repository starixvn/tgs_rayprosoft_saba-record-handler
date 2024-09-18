namespace SabaRecord.Core.Repositories;

public interface IApiLogsRepository
{
	ValueTask<bool> AddAsync(string action, string reqJson, string resJson, CancellationToken cancellationToken = default);
}