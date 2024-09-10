using Microsoft.Extensions.Logging;
using SabaRecord.Core.Constant;
using SabaRecord.Core.Models.Criteria.GameBet;
using SabaRecord.Core.Models.Enum;
using SabaRecord.Core.Repositories;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace SabaRecord.Core.ApplicationServices;

public class HandleBetCommandService(
	ILogger<HandleBetCommandService> logger,
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerRepository playerRepository,
	IGameBetRepository gameBetRepository,
	IGameRoundRepository roundRepository,
	IProviderRepository providerRepository
	) : ICommandService<HandleBetCommand>
{
	public async ValueTask ExecuteAsync(HandleBetCommand command, CancellationToken cancellationToken = default)
	{
		try
		{
			var gameProviderId = (Guid)await providerRepository.GetProviderIdByCodeAsync(command.GameProviderCode);
			var playerInfo = await playerRepository.GetPlayerInfobyPlayerCodeAsync(command.PlayerCode);
			var gameProviderConfigurationId = (Guid)await providerRepository.GetGameProviderConfigurationIdAsync(gameProviderId, playerInfo.BrandId);
			var gameInfo = await providerRepository.GetGamesByProviderIdAsync(gameProviderId);
			var exchangeRates = GetExchangeRates(playerInfo.PlayerCode, gameInfo.GameProviderCode);

			var existedRoundId = await roundRepository.GetIdAsync(gameProviderId, gameInfo.GameId, playerInfo.WalletId, command.ExternalRoundId);
			var id = existedRoundId.HasValue ? existedRoundId : await serialNumberGenerator.GenerateAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
			var roundStatus = command.BetState is BetState.Waiting or BetState.Running ? RoundStatus.Open : RoundStatus.Closed;

			// TO DO: Lack of Rounds Detail
			var roundCriteria = new Models.Criteria.GameRound.GameRoundSetCriteria()
			{
				Id = (Guid)id,
				ActionType = command.Action,
				GameProviderId = gameProviderId,
				GameProviderConfigurationId = gameProviderConfigurationId,
				GameId = gameInfo.GameId,
				BrandId = playerInfo.BrandId,
				PlayerCode = playerInfo.PlayerCode,
				CurrencyCode = playerInfo.CurrencyCode,
				WalletId = playerInfo.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalCreatedOn = command.ExternalCreatedOn,
				ExternalClosedOn = command.ExternalClosedOn,
				BetType = (short)command.BetType,
				BetAmount = ConvertPlatformRounds(exchangeRates, command.Bet),
				ActualAmount = ConvertPlatformRounds(exchangeRates, command.ActualAmount),
				ValidBetAmount = ConvertPlatformRounds(exchangeRates, command.ValidBet),
				SettleAmount = ConvertPlatformRounds(exchangeRates, command.Settle),
				Commission = ConvertPlatformRounds(exchangeRates, command.Commission),
				Turnover = ConvertPlatformRounds(exchangeRates, command.Turnover),
				Payout = ConvertPlatformRounds(exchangeRates, command.Payout),
				Status = (short)roundStatus,
				ClientStatus = (short)GetClientStatus(command.BetState),
				CreatedOn = DateTimeOffset.UtcNow.DateTime,
				UpdatedOn = DateTimeOffset.UtcNow.DateTime,
			};
			await roundRepository.GameRoundSetAsync(roundCriteria);

			// TO DO: Lack of Additional Data
			var gameBetCriteria = new GameBetSetCriteria()
			{
				GameProviderId = gameProviderId,
				GameId = gameInfo.GameId,
				WalletId = playerInfo.WalletId,
				ExternalBetId = command.ExternalBetId,
				ExternalCreatedOn = command.ExternalCreatedOn,
				Id = (Guid)id,
				GameProviderConfigurationId = gameProviderConfigurationId,
				ResellerId = playerInfo.ResellerId,
				LicenseeId = playerInfo.LicenseeId,
				BrandId = playerInfo.BrandId,
				PlayerCode = playerInfo.PlayerCode,
				CurrencyCode = playerInfo.CurrencyCode,
				ExternalRoundId = command.ExternalRoundId,
				ExternalClosedOn = command.ExternalClosedOn ?? DateTimeOffset.UnixEpoch.DateTime,
				ExternalPayoffOn = command.ExternalPayoffOn ?? DateTimeOffset.UnixEpoch.DateTime,
				ExternalResettleOn = command.ExternalResettleOn,
				BetState = (int)BetType.Normal,
				// TO DO: BetType is hard code
				BetType = (int)command.BetType,
				FreeGameState = (int)command.FreeGameState,
				GambleState = (int)command.GambleState,
				CashoutState = (int)command.CashoutState,
				ResettleState = (int)command.ResettleState,
				JackpotState = (int)command.JackpotState,
				CurrencyRatio = exchangeRates, // Double check
				Bet = ConvertPlatformRounds(exchangeRates, command.Bet),
				ActualAmount = ConvertPlatformRounds(exchangeRates, command.ActualAmount),
				ValidBet = ConvertPlatformRounds(exchangeRates, command.ValidBet),
				Settle = ConvertPlatformRounds(exchangeRates, command.Settle),
				Commission = ConvertPlatformRounds(exchangeRates, command.Commission),
				Turnover = ConvertPlatformRounds(exchangeRates, command.Turnover),
				Payout = ConvertPlatformRounds(exchangeRates, command.Payout),
				Jackpot = ConvertPlatformRounds(exchangeRates, command.Jackpot),
				ResetState = 0,
				CreatedOn = DateTimeOffset.UtcNow.DateTime,
				UpdatedOn = DateTimeOffset.UtcNow.DateTime,
				ClosedOn = DateTimeOffset.UnixEpoch.UtcDateTime,
				ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
				ActionType = command.Action
			};

			await gameBetRepository.GameBetSetAsync(gameBetCriteria);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			_ = SentrySdk.CaptureException(ex);
		}
	}

	private static RoundClientStatus GetClientStatus(BetState betState)
	{
		if (betState is BetState.Waiting or BetState.Running)
		{
			return RoundClientStatus.Open;
		}

		if (betState is BetState.Loss or BetState.HalfLoss)
		{
			return RoundClientStatus.Loss;
		}

		if (betState is BetState.Win or BetState.HalfWin)
		{
			return RoundClientStatus.Win;
		}

		if (betState is BetState.Draw)
		{
			return RoundClientStatus.Draw;
		}

		return RoundClientStatus.Void;
	}

	private static decimal ConvertPlatformRounds(decimal exchangeRate, decimal amount)
	=> amount == 0 ? amount : decimal.Round(amount * exchangeRate, 6, MidpointRounding.ToZero);

	private static decimal GetExchangeRates(string gameProviderCode, string currencyCode)
		=> GameCurrencySettings.GameProviderExchangeRates
			.Where(x => x.GameProviderCode.Equals(gameProviderCode, StringComparison.InvariantCultureIgnoreCase)
				&& x.CurrencyCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
			.Select(x => x.ExchangeRate)
			.SingleOrDefault(1);
}