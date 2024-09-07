using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaRecord.Core.ApplicationServices;

public class HandleBetCommand
{
	public string Action { get; set; }
	public string GameProviderCode { get; set; }
	public string PlayerCode { get; set; }
	public string ExternalRoundId { get; set; }
	public string ExternalBetId { get; set; }
	public string ExternalGameId { get; set; }
	public DateTime? ExternalCreatedOn { get; set; }
	public DateTime? ExternalClosedOn { get; set; }
	public DateTime? ExternalPayoffOn { get; set; }
	public BetState BetState { get; set; }
	public BetType BetType { get; set; }
	public FreeGameState FreeGameState { get; set; }
	public GambleState GambleState { get; set; }
	public CashoutState CashoutState { get; set; }
	public ResettleState ResettleState { get; set; }
	public JackpotState JackpotState { get; set; }
	public decimal Bet { get; set; }
	public decimal ValidBet { get; set; }
	public decimal Settle { get; set; }
	public decimal Commission { get; set; }
	public decimal Turnover { get; set; }
	public decimal WinLoss { get; set; }
	public decimal Jackpot { get; set; }
	public decimal ActualAmount { get; set; }
	public string AdditionalData { get; set; }
	public string ExtraStatus { get; set; }
	public long LicenseeTxid { get; set; }
}