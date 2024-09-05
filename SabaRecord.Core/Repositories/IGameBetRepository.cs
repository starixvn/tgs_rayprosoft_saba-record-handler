using SabaRecord.Core.Models.Criteria.GameBet;

namespace SabaRecord.Core.Repositories;

public interface IGameBetRepository
{
	Task GameBetSetAsync(GameBetSetCriteria criteria);
}