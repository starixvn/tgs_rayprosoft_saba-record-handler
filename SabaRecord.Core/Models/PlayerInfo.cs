namespace SabaRecord.Core.Models;
public class PlayerInfo
{
	public Guid ResellerId { get; set; }
	public string ResellerCode { get; set; }
	public Guid LicenseeId { get; set; }
	public string LicenseeCode { get; set; }
	public Guid BrandId { get; set; }
	public string BrandCode { get; set; }
	public string PlayerCode { get; set; }
	public string Username { get; set; }
	public Guid WalletId { get; set; }
	public string CurrencyCode { get; set; }
}