using Tgs.ServiceUtility;

namespace SabaRecord.Core.ApplicationServices;

public class ApiLogsCommandService(IApiLogsRepository logRepository) : ICommandService<ApiLogsCommand>
{
	public async ValueTask ExecuteAsync(ApiLogsCommand command, CancellationToken cancellationToken = default)
	{
		_ = await logRepository.AddAsync(command.Action, command.Req, command.Res, cancellationToken);
	}
}