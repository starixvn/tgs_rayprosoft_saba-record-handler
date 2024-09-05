using SabaRecord.Common;

namespace SabaRecord.Handler.Messages;

public interface ISapaApiMessagePublishHandler
{
	ValueTask LogApi(ApiLogMessage mesage, CancellationToken cancellationToken = default);

	ValueTask HandleBetAsync(HandleBetMessage mesage, CancellationToken cancellationToken = default);
}