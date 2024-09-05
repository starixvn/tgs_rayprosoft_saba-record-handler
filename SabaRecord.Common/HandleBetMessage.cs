namespace SabaRecord.Common;

public class HandleBetMessage
{
	public string Action { get; set; } = "";

	public string PlayerCode { get; set; } = "";

	public string ExtraStatus { get; set; } = "";

	public int BetState { get; set; } = 0;

	public DateTime WinlostDate { get; set; } = DateTimeOffset.UnixEpoch.Date;

	public DateTime TransactionTime { get; set; } = DateTimeOffset.UnixEpoch.Date;

	public decimal Settle { get; set; } = 0;

	public decimal BetAmount { get; set; } = 0;

	public decimal ActualAmount { get; set; } = 0;

	public decimal Payout { get; set; } = 0;

	public string TxId { get; set; } = "";

	public int BetType { get; set; } = 0;
}