using System.Text;
using System.Text.Json;
using SabaRecord.Common;
using SabaRecord.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaRecord.Handler.Messages;

public class ApiLogsMessageHandler(
	ILogger<ApiLogsMessageHandler> logger,
	IServiceProvider serviceProvider) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public async ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<ApiLogMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.Key,
					message.Action,
					message.Request,
					message.Response,
				},
				Message = "This message receiving handler."
			}));

		using var scope = serviceProvider.CreateScope();
		var service = scope.ServiceProvider.GetRequiredService<ICommandService<ApiLogsCommand>>();
		await service.ExecuteAsync(
			command: new ApiLogsCommand(message.Key, message.Action, message.Request, message.Response),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}