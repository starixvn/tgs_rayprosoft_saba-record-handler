namespace SabaRecord.Core.Configuarations
{
	public record NatsConfigs(string Subject, string StreamName, string ConsumerName, double? MaxAgeInMinute = null);
}