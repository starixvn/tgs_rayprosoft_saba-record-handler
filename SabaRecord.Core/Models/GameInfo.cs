namespace SabaRecord.Core.Models;
public class GameInfo
{
	public int GameId { get; set; }
	public string GameCode { get; set; }
	public Guid GameProviderId { get; set; }
	public string GameProviderCode { get; set; }
	public string ExternalGameId { get; set; }
	public string SubGameProviderCode { get; set; }
}