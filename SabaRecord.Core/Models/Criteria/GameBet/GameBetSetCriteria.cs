using System.Text.Json;

namespace SabaRecord.Core.Models.Criteria.GameBet;

public class GameBetSetCriteria
{
	public Guid GameProviderId { get; set; } // uuid
	public int GameId { get; set; } // integer
	public Guid WalletId { get; set; } // uuid
	public string ExternalBetId { get; set; } // character varying
	public DateTimeOffset? ExternalCreatedOn { get; set; } // timestamp with time zone
	public Guid Id { get; set; } // uuid
	public Guid GameProviderConfigurationId { get; set; } // uuid
	public Guid ResellerId { get; set; } // uuid
	public Guid LicenseeId { get; set; } // uuid
	public Guid BrandId { get; set; } // uuid
	public string PlayerCode { get; set; } // text
	public string CurrencyCode { get; set; } // character varying
	public string ExternalRoundId { get; set; } // character varying
	public DateTimeOffset? ExternalClosedOn { get; set; } // timestamp with time zone (nullable)
	public DateTimeOffset? ExternalPayoffOn { get; set; } // timestamp with time zone (nullable)
	public DateTimeOffset? ExternalResettleOn { get; set; } // timestamp with time zone (nullable)
	public int BetState { get; set; } // integer
	public int BetType { get; set; } // integer
	public int FreeGameState { get; set; } // integer
	public int GambleState { get; set; } // integer
	public int CashoutState { get; set; } // integer
	public int ResettleState { get; set; } // integer
	public int JackpotState { get; set; } // integer
	public decimal CurrencyRatio { get; set; } // numeric
	public decimal Bet { get; set; } // numeric
	public decimal ActualAmount { get; set; } // numeric
	public decimal ValidBet { get; set; } // numeric
	public decimal Settle { get; set; } // numeric
	public decimal Commission { get; set; } // numeric
	public decimal Turnover { get; set; } // numeric
	public decimal Payout { get; set; } // numeric
	public decimal Jackpot { get; set; } // numeric
	public DateTimeOffset? CreatedOn { get; set; } // timestamp with time zone
	public DateTimeOffset? ClosedOn { get; set; } // timestamp with time zone (nullable)
	public DateTimeOffset? UpdatedOn { get; set; } // timestamp with time zone (nullable)
	public int ResetState { get; set; } // integer
	public DateTimeOffset? ResetOn { get; set; } // timestamp with time zone (nullable)

	public string? AdditionalData { get; set; } // jsonb

	public string ActionType { get; set; } // character varying
}