

namespace SabaRecord.Core.Constant;
public static class GameCurrencySettings
{
	public static readonly List<GameProviderExchangeRate> GameProviderExchangeRates = new List<GameProviderExchangeRate>
	{
		new GameProviderExchangeRate
		{
			GameProviderCode = "AWC_PGS",
			CurrencyCode = "IDR",
			GameProviderCurrencyCode = "IDR",
			ExchangeRate = 1000
		},
		new GameProviderExchangeRate
		{
			GameProviderCode = "AWC_PGS",
			CurrencyCode = "VND",
			GameProviderCurrencyCode = "VND",
			ExchangeRate = 1000
		},
		new GameProviderExchangeRate
		{
			GameProviderCode = "BTI",
			CurrencyCode = "IDR",
			GameProviderCurrencyCode = "IDR",
			ExchangeRate = 1000
		},
		new GameProviderExchangeRate
		{
			GameProviderCode = "SABATF",
			CurrencyCode = "IDR",
			GameProviderCurrencyCode = "15",
			ExchangeRate = 1000
		}
	};
}

public class GameProviderExchangeRate
{
	public string CurrencyCode { get; set; } = default!;
	public decimal ExchangeRate { get; set; }
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderCurrencyCode { get; set; } = default!;
}
