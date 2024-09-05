using SabaRecord.Core.Models;

namespace SabaRecord.Core.Repositories;

public interface IPlayerRepository
{
	Task<PlayerInfo?> GetPlayerInfobyPlayerCodeAsync(string code);
}