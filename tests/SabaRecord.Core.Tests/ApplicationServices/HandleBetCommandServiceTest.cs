using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SabaRecord.Core.ApplicationServices;
using SabaRecord.Core.Constant;
using SabaRecord.Core.Models;
using SabaRecord.Core.Models.Criteria.GameRound;
using SabaRecord.Core.Repositories;
using Tgs.SerialNumberGenerator;

namespace SabaRecord.Core.Tests.ApplicationServices;

public class HandleBetCommandServiceTest
{
	private Fixture fixture;

	private ILogger<HandleBetCommandService> logger = Substitute.For<ILogger<HandleBetCommandService>>();

	private ISerialNumberGenerator serialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

	private IPlayerRepository playerRepository = Substitute.For<IPlayerRepository>();

	private IGameRoundRepository roundRepository = Substitute.For<IGameRoundRepository>();

	private IProviderRepository providerRepository = Substitute.For<IProviderRepository>();

	private IGameCurrencyRepository gameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();

	private HandleBetCommandService handleBetCommandService;

	public HandleBetCommandServiceTest()
	{
		fixture = new Fixture();

		handleBetCommandService = new HandleBetCommandService(logger, serialNumberGenerator, playerRepository, roundRepository, providerRepository, gameCurrencyRepository);
	}

	[Theory]
	[InlineData(1.234567, 100.123456789, 123.609115)] // Amount With More Than Six Decimal Places
	[InlineData(1.123456, -100, -112.345600)] // Negative Amount => Return Negative Amount
	[InlineData(1.5, 0, 0)] // Amount Is Zero => Return Zero
	public async Task ExecuteAsync_WhenCall_ReturnCorrectExchangeRate(decimal exchangeRates, decimal amount, decimal result)
	{
		// Arange
		var command = fixture.Create<HandleBetCommand>();
		command.Action = HandleAction.ConfirmBet;
		command.Bet = amount;
		command.ActualAmount = amount;
		command.Payout = amount;

		var gameProviderId = fixture.Create<Guid>();
		providerRepository.GetProviderIdByCodeAsync(command.GameProviderCode).Returns(gameProviderId);

		var playerInfo = fixture.Create<PlayerInfo>();
		playerRepository.GetPlayerInfobyPlayerCodeAsync(command.PlayerCode).Returns(playerInfo);

		var gameProviderConfigurationId = fixture.Create<Guid>();
		providerRepository.GetGameProviderConfigurationIdAsync(gameProviderId, playerInfo.BrandId).Returns(gameProviderConfigurationId);

		var gameInfo = fixture.Create<GameInfo>();
		providerRepository.GetGamesByProviderIdAsync(gameProviderId).Returns(gameInfo);

		gameCurrencyRepository.GetExchangeRates(playerInfo.PlayerCode, gameInfo.GameProviderCode).Returns(exchangeRates);

		var existedRoundId = fixture.Create<Guid>();
		roundRepository.GetIdAsync(gameProviderId, gameInfo.GameId, playerInfo.WalletId, command.ExternalRoundId).Returns(existedRoundId);

		// Action
		await handleBetCommandService.ExecuteAsync(command, default);

		// Assert
		await roundRepository.Received(1).ConfirmBetAsync(Arg.Is<GameRoundSetCriteria>(criteria => criteria.BetAmount == result
																									&& criteria.ActualAmount == result
																									&& criteria.Payout == result));
	}
}