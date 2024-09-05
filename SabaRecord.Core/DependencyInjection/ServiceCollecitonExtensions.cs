using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SabaRecord.Core.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddSabaRecordHandlerApiCore(
	this IServiceCollection services)
	=> services
	.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
	.AddSerialNumberGenerator()
	.AddSingleton<IApiLogsDispatcher, ApiLogsDispatcher>()
	.AddSingleton<IHandleBetDispatcher, HandleBetDispatcher>();
}