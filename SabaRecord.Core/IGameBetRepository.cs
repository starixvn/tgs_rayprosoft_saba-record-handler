using SabaRecord.Core.Models.Criteria.GameBet;

namespace SabaRecord.Core;

public interface IGameBetRepository
{
	Task GameBetSetAsync(GameBetSetCriteria criteria);
}