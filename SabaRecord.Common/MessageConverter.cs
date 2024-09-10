using System.Text.Json;
using Microsoft.Extensions.Options;
using SabaRecord.Core.ApplicationServices;
using SabaRecord.Core.Configuarations;
using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaRecord.Common;

public interface IMessageConverter
{
	HandleBetCommand ToHandleBetCommand(HandleBetMessage message);
}

public class MessageConverter(IOptions<GameProviderInfo> gameRecordInfoOptions) : IMessageConverter
{
	private readonly GameProviderInfo _gameRecordInfoOptions = gameRecordInfoOptions.Value;

	public HandleBetCommand ToHandleBetCommand(HandleBetMessage message)
	{
		var betState = (BetState)message.BetState;
		var isClosed = betState is not BetState.Running;
		var isVoid = betState is BetState.Void or BetState.Running or BetState.Refund or BetState.Refund;
		var isCashout = !string.IsNullOrEmpty(message.ExtraStatus) || message.ExtraStatus.Equals("void cashout", StringComparison.OrdinalIgnoreCase);

		var bet = message.BetAmount;
		var validBet = message.BetAmount;

		var settle = 0.0m;
		var turnover = 0.0m;
		var payout = 0.0m;
		var cashoutState = CashoutState.None;
		var externalClosedOn = (DateTimeOffset?)null;

		if (isClosed)
		{
			settle = message.Settle;
			payout = message.Payout;
			externalClosedOn = message.WinlostDate?.ToUniversalTime() ?? DateTimeOffset.UtcNow;

			validBet = isCashout ? 0 : !isVoid ? message.BetAmount : 0;
			turnover = isCashout ? 0 : !isVoid ? message.BetAmount : 0;
			cashoutState = isCashout ? CashoutState.Cashout : CashoutState.None;
		}

		return new HandleBetCommand
		{
			Action = message.Action,
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			ExternalGameId = _gameRecordInfoOptions.GameExternalId,
			PlayerCode = message.PlayerCode,
			ExternalBetId = message.TxId,
			ExternalRoundId = message.TxId,
			ExternalCreatedOn = message.TransactionTime ?? DateTime.UtcNow,
			ExternalClosedOn = externalClosedOn?.DateTime,
			ExternalPayoffOn = externalClosedOn?.DateTime,
			// TO DO: ExternalResettleOn is hard code
			ExternalResettleOn = DateTimeOffset.UnixEpoch.DateTime,
			BetState = betState,
			BetType = (BetType)message.BetType,
			FreeGameState = FreeGameState.None,
			GambleState = GambleState.None,
			CashoutState = cashoutState,
			ResettleState = ResettleState.None,
			JackpotState = JackpotState.None,
			Bet = bet,
			ActualAmount = message.ActualAmount,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			Payout = payout,
			Jackpot = 0.0M,
			AdditionalData = JsonSerializer.Serialize(message)
		};
	}
}