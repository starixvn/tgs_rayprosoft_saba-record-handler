using Microsoft.Extensions.Options;
using SabaRecord.Core.Configuarations;
using SabaRecord.Core.Repositories;

namespace SabaRecord.Repositories;

public class GameCurrencyRepository(IOptions<GameCurrencySettings> gameCurrencySettings) : IGameCurrencyRepository
{
	public GameCurrencySettings GameCurrencySettings { get; } = gameCurrencySettings.Value;

	public decimal GetExchangeRates(string gameProviderCode, string currencyCode) => GameCurrencySettings.GameProviderExchangeRates
	.Where(x => x.GameProviderCode.Equals(gameProviderCode, StringComparison.InvariantCultureIgnoreCase) && x.CurrencyCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
	.Select(x => x.ExchangeRate)
	.SingleOrDefault(1);
}