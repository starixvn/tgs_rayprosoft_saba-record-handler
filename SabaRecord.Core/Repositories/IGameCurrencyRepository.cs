namespace SabaRecord.Core.Repositories;

public interface IGameCurrencyRepository
{
	decimal GetExchangeRates(string gameProviderCode, string currencyCode);
}