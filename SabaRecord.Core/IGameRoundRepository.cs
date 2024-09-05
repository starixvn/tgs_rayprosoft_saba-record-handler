using SabaRecord.Core.Models.Criteria.GameRound;

namespace SabaRecord.Repositories;

public interface IGameRoundRepository
{
	Task<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId);

	Task GameRoundSetAsync(GameRoundSetCriteria criteria);

	Task GameRoundDetailSetAsync(GameRoundDetailSetCriteria criteria);
}