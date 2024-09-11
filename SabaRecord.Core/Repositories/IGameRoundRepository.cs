using SabaRecord.Core.Models.Criteria.GameRound;

namespace SabaRecord.Core.Repositories;

public interface IGameRoundRepository
{
	Task<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId);

	Task ConfirmBetAsync(GameRoundSetCriteria criteria);

	Task SettleAsync(GameRoundSetCriteria criteria);

	Task ResettleAsync(GameRoundSetCriteria criteria);
}