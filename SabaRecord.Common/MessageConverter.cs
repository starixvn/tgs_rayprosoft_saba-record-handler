using System.Text.Json;
using Microsoft.Extensions.Options;
using SabaRecord.Common;
using SabaRecord.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaRecord.Core;

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
		var idVoid = betState is BetState.Void or BetState.Running or BetState.Refund or BetState.Refund;
		var isCashOut = message.ExtraStatus.Equals("cashout", StringComparison.OrdinalIgnoreCase);

		var bet = message.BetAmount;
		var validBet = isCashOut ? 0 : !idVoid ? message.BetAmount : 0;

		var settle = 0.0m;
		var turnover = 0.0m;
		var winLoss = 0.0m;
		var cashoutState = CashoutState.None;
		var externalClosedOn = DateTimeOffset.UnixEpoch.DateTime;

		if (isClosed)
		{
			turnover = message.BetAmount;
			settle = message.Settle;
			externalClosedOn = message.WinlostDate;
			winLoss = message.WinLoss;
			cashoutState = isCashOut ? CashoutState.Cashout : CashoutState.None;
		}

		return new HandleBetCommand
		{
			Action = message.Action,
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			ExternalGameId = _gameRecordInfoOptions.GameExternalId,
			PlayerCode = message.PlayerCode,
			ExternalBetId = message.TxId,
			ExternalRoundId = message.TxId,
			ExternalCreatedOn = message.TransactionTime,
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch.DateTime,
			BetState = betState,
			BetType = BetType.Normal,
			FreeGameState = FreeGameState.None,
			GambleState = GambleState.None,
			CashoutState = cashoutState,
			ResettleState = ResettleState.None,
			JackpotState = JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = JsonSerializer.Serialize(message)
		};
	}
}