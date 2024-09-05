using SabaRecord.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCommon(
		this IServiceCollection services)
		=> services
		.AddTransient<IMessageConverter, MessageConverter>();
}