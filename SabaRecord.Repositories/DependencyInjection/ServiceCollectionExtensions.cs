using Microsoft.Extensions.DependencyInjection;
using SabaRecord.Core.Repositories;

namespace SabaRecord.Repositories.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRepositories(this IServiceCollection services)
		   => services.AddScoped<IPlayerRepository, PlayerRepositoryImp>()
		.AddScoped<IProviderRepository, ProviderRepository>()
		.AddScoped<IGameRoundRepository, GameRoundRepository>();
}