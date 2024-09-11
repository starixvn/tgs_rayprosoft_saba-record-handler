using Microsoft.Extensions.Options;
using NSubstitute;
using SabaRecord.Core.Configuarations;
using SabaRecord.Core.Models;

namespace SabaRecord.Repositories.Tests;

public class GameCurrencyRepositoryTest
{
	private readonly IOptions<GameCurrencySettings> _mockSettings;
	private readonly List<GameProviderExchangeRate> _exchangeRates;
	private readonly GameCurrencyRepository _repository;

	public GameCurrencyRepositoryTest()
	{
		_exchangeRates =
		[
			new() { GameProviderCode = "ProviderA", CurrencyCode = "USD", ExchangeRate = 1.2m },
			new() { GameProviderCode = "ProviderA", CurrencyCode = "EUR", ExchangeRate = 0.9m },
			new() { GameProviderCode = "ProviderB", CurrencyCode = "USD", ExchangeRate = 1.5m },
		];

		var mockGameCurrencySettings = new GameCurrencySettings
		{
			GameProviderExchangeRates = _exchangeRates
		};

		_mockSettings = Substitute.For<IOptions<GameCurrencySettings>>();

		_mockSettings.Value.Returns(mockGameCurrencySettings);

		_repository = new GameCurrencyRepository(_mockSettings);
	}

	[Fact]
	public void GetExchangeRates_ShouldReturnCorrectExchangeRate_WhenValidInputs()
	{
		// Act
		var result = _repository.GetExchangeRates("ProviderA", "USD");

		// Assert
		Assert.Equal(1.2m, result);
	}

	[Fact]
	public void GetExchangeRates_ShouldReturnDefaultValue_WhenInvalidProviderCode()
	{
		// Act
		var result = _repository.GetExchangeRates("ProviderX", "USD");

		// Assert
		Assert.Equal(1, result);
	}

	[Fact]
	public void GetExchangeRates_ShouldReturnDefaultValue_WhenInvalidCurrencyCode()
	{
		// Act
		var result = _repository.GetExchangeRates("ProviderA", "JPY");

		// Assert
		Assert.Equal(1, result);
	}

	[Fact]
	public void GetExchangeRates_ShouldBeCaseInsensitive_WhenComparingProviderAndCurrencyCodes()
	{
		// Act
		var result = _repository.GetExchangeRates("providera", "usd"); // Lowercase input

		// Assert
		Assert.Equal(1.2m, result);
	}
}