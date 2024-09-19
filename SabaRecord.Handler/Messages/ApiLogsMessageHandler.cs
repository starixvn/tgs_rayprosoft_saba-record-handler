using System.Text;
using Newtonsoft.Json;
using SabaRecord.Common;
using SabaRecord.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaRecord.Handler.Messages;

public class ApiLogsMessageHandler(
	ILogger<ApiLogsMessageHandler> logger,
	IServiceProvider serviceProvider,
	ISentryClient sentryClient) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public async ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		try
		{
			var json = Encoding.UTF8.GetString(data.Span);

			var logData = JsonConvert.DeserializeObject<ApiLogMessage>(json)
				?? throw new NullReferenceException("The request data resolves to a null value");

			var key = !string.IsNullOrWhiteSpace(logData.Key) ? logData.Key : (string)logData.Request["key"];
			var action = !string.IsNullOrWhiteSpace(logData.Action) ? logData.Action : (string)logData.Request.Message.Action;
			var request = logData.Request;
			var response = logData.Response;

			logger.LogInformation("{logInformation}",
				JsonConvert.SerializeObject(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Subject = subject,
					Data = new
					{
						key,
						action,
						request,
						response,
					},
					Message = "This logData receiving handler."
				}));

			using var scope = serviceProvider.CreateScope();
			var service = scope.ServiceProvider.GetRequiredService<ICommandService<ApiLogsCommand>>();
			var command = new ApiLogsCommand(key, action, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));

			await service.ExecuteAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);
		}
	}
}