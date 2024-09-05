using SabaRecord.Core.ApplicationServices;

namespace SabaRecord.Core;

public interface IApiLogsDispatcher
{
	ValueTask PushCommandAsync(ApiLogsCommand command, CancellationToken cancellationToken = default);
}