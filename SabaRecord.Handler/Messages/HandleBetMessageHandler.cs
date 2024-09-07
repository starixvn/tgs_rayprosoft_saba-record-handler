using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SabaRecord.Common;
using SabaRecord.Core;
using SabaRecord.Core.ApplicationServices;
using SabaRecord.Core.Configuarations;
using Tgs.GameRecord.Handle.Common.Protos;
using Valhalla.MessageQueue;

namespace SabaRecord.Handler.Messages;

public class HandleBetMessageHandler(
	ILogger<HandleBetMessageHandler> logger,
	IOptions<GameProviderInfo> gameRecordInfoOptions,
	IHandleBetDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	private readonly GameProviderInfo _gameRecordInfoOptions = gameRecordInfoOptions.Value;

	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<HandleBetMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					message.Action,
					message.PlayerCode,
					message.ExtraStatus,
					message.BetState,
					message.WinlostDate,
					message.TransactionTime,
					message.WinLoss,
					message.TxId
				},
				Message = "This message receiving handler."
			}));

		return dispatcher.PushCommandAsync(
			command: ConvertToHandleBetCommand(message),
			cancellationToken: cancellationToken);
	}

	public HandleBetCommand ConvertToHandleBetCommand(HandleBetMessage message)
	{
		var externalClosedOn = message.WinlostDate?.ToUniversalTime();

		return new HandleBetCommand
		{
			Action = message.Action,
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			ExternalGameId = _gameRecordInfoOptions.GameExternalId,
			PlayerCode = message.PlayerCode,
			ExternalBetId = message.TxId,
			ExternalRoundId = message.TxId,
			ExternalCreatedOn = message.TransactionTime?.ToUniversalTime(),
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExtraStatus = message.ExtraStatus,
			WinLoss = message.WinLoss,
			ActualAmount = message.ActualAmount,
			LicenseeTxid = message.LicenseeTxid,
			BetState = (BetState)message.BetState,
			FreeGameState = FreeGameState.None,
			GambleState = GambleState.None,
			ResettleState = ResettleState.None,
			JackpotState = JackpotState.None,
			AdditionalData = JsonSerializer.Serialize(message)
		};
	}
}