using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaRecord.Core;
using SabaRecord.Core.Models.Criteria.GameBet;

namespace SabaRecord.Repositories;

public class GameBetRepository : IGameBetRepository
{
	private readonly string _connectionString;

	public GameBetRepository(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("Tgs")!;
	}

	public async Task GameBetSetAsync(GameBetSetCriteria criteria)
	{
		using (IDbConnection db = new NpgsqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();

			parameters.Add("@game_provider_id", criteria.GameProviderId);
			parameters.Add("@game_id", criteria.GameId);
			parameters.Add("@wallet_id", criteria.WalletId);
			parameters.Add("@external_bet_id", criteria.ExternalBetId);
			parameters.Add("@external_created_on", criteria.ExternalCreatedOn);
			parameters.Add("@id", criteria.Id);
			parameters.Add("@game_provider_configuration_id", criteria.GameProviderConfigurationId);
			parameters.Add("@reseller_id", criteria.ResellerId);
			parameters.Add("@licensee_id", criteria.LicenseeId);
			parameters.Add("@brand_id", criteria.BrandId);
			parameters.Add("@player_code", criteria.PlayerCode);
			parameters.Add("@currency_code", criteria.CurrencyCode);
			parameters.Add("@external_round_id", criteria.ExternalRoundId);
			parameters.Add("@external_closed_on", criteria.ExternalClosedOn);
			parameters.Add("@external_payoff_on", criteria.ExternalPayoffOn);
			parameters.Add("@external_resettle_on", criteria.ExternalResettleOn);
			parameters.Add("@bet_state", criteria.BetState);
			parameters.Add("@bet_type", criteria.BetType);
			parameters.Add("@free_game_state", criteria.FreeGameState);
			parameters.Add("@gamble_state", criteria.GambleState);
			parameters.Add("@cashout_state", criteria.CashoutState);
			parameters.Add("@resettle_state", criteria.ResettleState);
			parameters.Add("@jackpot_state", criteria.JackpotState);
			parameters.Add("@currency_ratio", criteria.CurrencyRatio);
			parameters.Add("@bet", criteria.Bet);
			parameters.Add("@valid_bet", criteria.ValidBet);
			parameters.Add("@settle", criteria.Settle);
			parameters.Add("@commission", criteria.Commission);
			parameters.Add("@turnover", criteria.Turnover);
			parameters.Add("@payout", criteria.WinLoss);
			parameters.Add("@jackpot", criteria.Jackpot);
			parameters.Add("@created_on", criteria.CreatedOn);
			parameters.Add("@closed_on", criteria.ClosedOn);
			parameters.Add("@updated_on", criteria.UpdatedOn);
			parameters.Add("@reset_state", criteria.ResetState);
			parameters.Add("@reset_on", criteria.ResetOn);
			parameters.Add("@actiontype", criteria.ActionType);

			string sqlQuery = "CALL dbo.game_bets_set(" +
		  "game_provider_id => @game_provider_id, " +
		  "game_id => @game_id, " +
		  "wallet_id => @wallet_id, " +
		  "external_bet_id => @external_bet_id, " +
		  "external_created_on => @external_created_on, " +
		  "id => @id, " +
		  "game_provider_configuration_id => @game_provider_configuration_id, " +
		  "reseller_id => @reseller_id, " +
		  "licensee_id => @licensee_id, " +
		  "brand_id => @brand_id, " +
		  "player_code => @player_code, " +
		  "currency_code => @currency_code, " +
		  "external_round_id => @external_round_id, " +
		  "external_closed_on => @external_closed_on, " +
		  "external_payoff_on => @external_payoff_on, " +
		  "external_resettle_on => @external_resettle_on, " +
		  "bet_state => @bet_state, " +
		  "bet_type => @bet_type, " +
		  "free_game_state => @free_game_state, " +
		  "gamble_state => @gamble_state, " +
		  "cashout_state => @cashout_state, " +
		  "resettle_state => @resettle_state, " +
		  "jackpot_state => @jackpot_state, " +
		  "currency_ratio => @currency_ratio, " +
		  "bet => @bet, " +
		  "valid_bet => @valid_bet, " +
		  "settle => @settle, " +
		  "commission => @commission, " +
		  "turnover => @turnover, " +
		  "payout => @payout, " +
		  "jackpot => @jackpot, " +
		  "created_on => @created_on, " +
		  "closed_on => @closed_on, " +
		  "updated_on => @updated_on, " +
		  "reset_state => @reset_state, " +
		  "reset_on => @reset_on, " +
		  "actiontype => @actiontype)";

			await db.ExecuteAsync(sqlQuery, parameters);

			return;
		}
	}
}