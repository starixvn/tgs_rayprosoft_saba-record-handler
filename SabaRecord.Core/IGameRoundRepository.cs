using SabaRecord.Core.Models.Criteria.GameRound;

namespace SabaRecord.Repositories;

public interface IGameRoundRepository
{
	Task<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId);

	Task<decimal> GameRoundSetAsync(GameRoundSetCriteria criteria);

	Task<decimal> GameRoundDetailSetAsync(GameRoundDetailSetCriteria criteria);
}