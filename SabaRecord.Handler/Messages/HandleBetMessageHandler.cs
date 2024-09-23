using System.Text;
using System.Text.Json;
using SabaRecord.Common;
using SabaRecord.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaRecord.Handler.Messages;

public class HandleBetMessageHandler(
	ILogger<HandleBetMessageHandler> logger,
	ICommandService<HandleBetCommand> handleBetCommand) : IAcknowledgeMessageHandler<ReadOnlyMemory<byte>>
{
	public async ValueTask HandleAsync(IAcknowledgeMessage<ReadOnlyMemory<byte>> msg, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(msg.Data.Span);
		var message = JsonSerializer.Deserialize<HandleBetMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = msg.Subject,
				Data = new
				{
					message.Action,
					message.PlayerCode,
					message.ExtraStatus,
					message.BetState,
					message.WinlostDate,
					message.TransactionTime,
					message.Payout,
					message.TxId,
					message.RefId
				},
				Message = "This message receiving handler."
			}));

		await handleBetCommand.ExecuteAsync(
			command: ConvertToHandleBetCommand(message),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await msg.AckAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public HandleBetCommand ConvertToHandleBetCommand(HandleBetMessage message)
	{
		var externalClosedOn = message.WinlostDate?.ToUniversalTime();

		return new HandleBetCommand
		{
			Action = message.Action,
			GameProviderCode = message.GameProviderCode,
			ExternalGameId = message.GameExternalId,
			PlayerCode = message.PlayerCode,
			ExternalBetId = message.TxId,
			RefId = message.RefId,
			ExternalRoundId = message.TxId,
			ExternalCreatedOn = message.TransactionTime?.ToUniversalTime(),
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExtraStatus = message.ExtraStatus,
			Payout = message.Payout,
			ActualAmount = message.ActualAmount,
			LicenseeTxid = message.LicenseeTxid,
			BetState = message.BetState,
			FreeGameState = FreeGameState.None,
			GambleState = GambleState.None,
			ResettleState = ResettleState.None,
			JackpotState = JackpotState.None,
			AdditionalData = JsonSerializer.Serialize(message),
			IsOnlyWinlostDateChanged = message.IsOnlyWinlostDateChanged
		};
	}
}