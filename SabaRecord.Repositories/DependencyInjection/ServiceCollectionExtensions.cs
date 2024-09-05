using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SabaRecord.Core;

namespace SabaRecord.Repositories.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApiLogsRepositories(
		this IServiceCollection services,
		Func<IServiceProvider, IMongoClient> mongoClientResolver)
		=> services
		.AddTransient<IApiLogsRepository>((sp) => ActivatorUtilities.CreateInstance<ApiLogsRepository>(sp, mongoClientResolver(sp)));

	public static IServiceCollection AddRepositories(this IServiceCollection services)
		   => services.AddScoped<IPlayerRepository, PlayerRepositoryImp>()
		.AddScoped<IGameBetRepository, GameBetRepository>()
		.AddScoped<IGameRoundRepository, GameRoundRepository>();
}