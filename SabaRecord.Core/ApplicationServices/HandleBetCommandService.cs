using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SabaRecord.Core.Constant;
using SabaRecord.Core.Models.Criteria.GameRound;
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
			var roundCriteria = new GameRoundSetCriteria()
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
				Status = (short)roundStatus,
				BetAmount = ConvertPlatformRounds(exchangeRates, command.Bet),
				ActualAmount = ConvertPlatformRounds(exchangeRates, command.ActualAmount),
				Payout = ConvertPlatformRounds(exchangeRates, command.WinLoss),
				ClientStatus = (short)GetClientStatus(command.BetState),
				CreatedOn = DateTimeOffset.UtcNow.DateTime,
				UpdatedOn = DateTimeOffset.UtcNow.DateTime,
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			_ = SentrySdk.CaptureException(ex);
		}
	}

	private async Task ProcessHandleAsync(string action, GameRoundSetCriteria criteria)
	{
		switch (action)
		{
			case HandleAction.ConfirmBet:
				await roundRepository.ConfirmBetAsync(criteria);
				return;

			case HandleAction.Settle:
			case HandleAction.Unsettle:
			case HandleAction.Resettle:
				await roundRepository.SettleAsync(criteria);
				return;

			default:
				throw new InvalidOperationException($"Invalid Action: {action}");
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