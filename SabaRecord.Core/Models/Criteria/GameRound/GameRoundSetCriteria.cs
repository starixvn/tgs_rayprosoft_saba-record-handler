namespace SabaRecord.Core.Models.Criteria.GameRound;

public class GameRoundSetCriteria
{
	public Guid Id { get; set; } // uuid
	public DateTime CreatedOn { get; set; } // timestamp with time zone
	public string ExternalRoundId { get; set; } // character varying
	public Guid GameProviderId { get; set; } // uuid
	public string PlayerCode { get; set; } // text
	public int GameId { get; set; } // integer
	public string CurrencyCode { get; set; } // character varying
	public short Status { get; set; } // smallint
	public Guid GameProviderConfigurationId { get; set; } // uuid
	public Guid WalletId { get; set; } // uuid
	public DateTime? ClosedOn { get; set; } // timestamp with time zone (nullable)
	public DateTime? UpdatedOn { get; set; } // timestamp with time zone (nullable)
	public Guid BrandId { get; set; } // uuid
	public DateTime ExternalCreatedOn { get; set; } // timestamp with time zone
	public DateTime? ExternalClosedOn { get; set; } // timestamp with time zone (nullable)
	public decimal BetAmount { get; set; } // numeric
	public decimal ValidBetAmount { get; set; } // numeric
	public decimal SettleAmount { get; set; } // numeric
	public decimal Commission { get; set; } // numeric
	public decimal Turnover { get; set; } // numeric
	public decimal WinLoss { get; set; } // numeric
	public decimal TieBetAdjustment { get; set; } // numeric
	public short ClientStatus { get; set; } // smallint
	public decimal ReopenTurnoverAdjustment { get; set; } // numeric
	public DateTime? ReopenedOn { get; set; } // timestamp with time zone (nullable)
	public decimal EffectiveBettingPercentage { get; set; } // numeric
	public string ActionType { get; set; } // character varying
}