using SabaRecord.Core.ApplicationServices;

namespace SabaRecord.Core;

public interface IHandleBetDispatcher
{
	ValueTask PushCommandAsync(HandleBetCommand command, CancellationToken cancellationToken = default);
}