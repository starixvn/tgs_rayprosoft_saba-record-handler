using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabaRecord.Core.Models;

public class GameProviderExchangeRate
{
	public string CurrencyCode { get; set; } = default!;
	public decimal ExchangeRate { get; set; }
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderCurrencyCode { get; set; } = default!;
}