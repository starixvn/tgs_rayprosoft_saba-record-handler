namespace SabaRecord.Handler.Extensions;

public static class UserIdConverter
{
	public static string ToPlayerCode(this string userId)
	{
		return userId.Split("_")[1];
	}
}