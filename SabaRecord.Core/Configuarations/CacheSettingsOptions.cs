namespace SabaRecord.Core.Configuarations;
public record CacheSettingsOptions
{
	public int ExpiredTimeInMinute { get; init; } = 1;

	public int ExpiredTimeInHour { get; init; } = 1;
}