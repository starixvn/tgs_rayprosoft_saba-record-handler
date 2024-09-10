namespace SabaRecord.Common;

public class HandleBetMessage
{
	public string Action { get; set; } = "";

	public string PlayerCode { get; set; } = "";

	public string ExtraStatus { get; set; } = "";

	public int BetState { get; set; } = 0;

	public DateTime? WinlostDate { get; set; }

	public DateTime? TransactionTime { get; set; }

	public decimal ActualAmount { get; set; } = 0;

	public decimal Payout { get; set; } = 0;

	public string TxId { get; set; } = "";

	public string? RefId { get; set; } = "";

	public long LicenseeTxid { get; set; }
}