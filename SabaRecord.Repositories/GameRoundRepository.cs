using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaRecord.Core.Models.Criteria;
using SabaRecord.Core.Models.Criteria.GameRound;
using SabaRecord.Core.Repositories;

namespace SabaRecord.Repositories;

public class GameRoundRepository : IGameRoundRepository
{
	private readonly string spName = "bet.bet_rounds_set";
	private readonly string _connectionString;

	public GameRoundRepository(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("Tgs")!;
	}

	public async Task<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId)
	{
		using (IDbConnection db = new NpgsqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();

			parameters.Add("@external_round_id", externalRoundId);
			parameters.Add("@game_provider_id", gameProviderId);
			parameters.Add("@game_id", gameId);
			parameters.Add("@wallet_id", walletId);

			string sqlQuery = @"SELECT r.id
								FROM bet.rounds r
								WHERE r.external_round_id = @external_round_id
								  AND r.game_provider_id = @game_provider_id
								  AND r.game_id = @game_id
								  AND r.wallet_id = @wallet_id;";

			var ids = await db.QueryFirstOrDefaultAsync<Guid?>(sqlQuery, parameters); // check correct property later

			return ids;
		}
	}

	public async Task ConfirmBetAsync(GameRoundSetCriteria criteria)
	{
		try
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@id", criteria.Id);
				parameters.Add("@external_round_id", criteria.ExternalRoundId);
				parameters.Add("@game_provider_id", criteria.GameProviderId);
				parameters.Add("@player_code", criteria.PlayerCode);
				parameters.Add("@game_id", criteria.GameId);
				parameters.Add("@currency_code", criteria.CurrencyCode);
				parameters.Add("@game_provider_configuration_id", criteria.GameProviderConfigurationId);
				parameters.Add("@wallet_id", criteria.WalletId);
				parameters.Add("@brand_id", criteria.BrandId);
				parameters.Add("@licenseetxid", criteria.LicenseeTxId);
				parameters.Add("@external_created_on", criteria.ExternalCreatedOn);
				parameters.Add("@actiontype", criteria.ActionType);

				string sqlQuery = $"CALL {spName}(" +
						"id => @id, " +
						"external_created_on => @external_created_on, " +
						"external_round_id => @external_round_id, " +
						"game_provider_id => @game_provider_id, " +
						"player_code => @player_code, " +
						"game_id => @game_id, " +
						"currency_code => @currency_code, " +
						"game_provider_configuration_id => @game_provider_configuration_id, " +
						"wallet_id => @wallet_id, " +
						"brand_id => @brand_id, " +
						"licenseetxid => @licenseetxid, " +
						"actiontype => @actiontype);";

				await db.QueryFirstAsync<DbExcutionResult>(sqlQuery, parameters);
			}
		}
		catch (Exception ex) { 
			SentrySdk.CaptureException(ex);
			Console.WriteLine(ex.ToString());
		}
		
	}

	public async Task SettleAsync(GameRoundSetCriteria criteria)
	{
		using (IDbConnection db = new NpgsqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();

			parameters.Add("@id", criteria.Id);
			parameters.Add("@external_round_id", criteria.ExternalRoundId);
			parameters.Add("@game_provider_id", criteria.GameProviderId);
			parameters.Add("@player_code", criteria.PlayerCode);
			parameters.Add("@game_id", criteria.GameId);
			parameters.Add("@currency_code", criteria.CurrencyCode);
			parameters.Add("@status", criteria.Status);
			parameters.Add("@game_provider_configuration_id", criteria.GameProviderConfigurationId);
			parameters.Add("@wallet_id", criteria.WalletId);
			parameters.Add("@closed_on", criteria.ClosedOn);
			parameters.Add("@brand_id", criteria.BrandId);
			parameters.Add("@external_created_on", criteria.ExternalCreatedOn);
			parameters.Add("@external_closed_on", criteria.ExternalClosedOn);
			parameters.Add("@bet_type", criteria.BetType);
			parameters.Add("@bet_amount", criteria.BetAmount);
			parameters.Add("@actual_bet_amount", criteria.ActualAmount);
			parameters.Add("@valid_bet_amount", criteria.ValidBetAmount);
			parameters.Add("@settle_amount", criteria.SettleAmount);
			parameters.Add("@commission", criteria.Commission);
			parameters.Add("@turnover", criteria.Turnover);
			parameters.Add("@payout", criteria.Payout);
			parameters.Add("@tie_bet_adjustment", criteria.TieBetAdjustment);
			parameters.Add("@client_status", criteria.ClientStatus);
			parameters.Add("@reopen_turnover_adjustment", criteria.ReopenTurnoverAdjustment);
			parameters.Add("@reopened_on", criteria.ReopenedOn);
			parameters.Add("@effective_betting_percentage", criteria.EffectiveBettingPercentage);
			parameters.Add("@actiontype", criteria.ActionType);
			parameters.Add("@updated_on", criteria.UpdatedOn);
			parameters.Add("@created_on", criteria.CreatedOn);

			string sqlQuery = $"CALL {spName}(" +
						"id => @id, " +
						"created_on => @created_on, " +
						"external_round_id => @external_round_id, " +
						"game_provider_id => @game_provider_id, " +
						"player_code => @player_code, " +
						"game_id => @game_id, " +
						"currency_code => @currency_code, " +
						"status => @status, " +
						"game_provider_configuration_id => @game_provider_configuration_id, " +
						"wallet_id => @wallet_id, " +
						"closed_on => @closed_on, " +
						"updated_on => @updated_on, " +
						"brand_id => @brand_id, " +
						"external_created_on => @external_created_on, " +
						"external_closed_on => @external_closed_on, " +
						"bet_type => @bet_type, " +
						"bet_amount => @bet_amount, " +
						"actual_bet_amount => @actual_bet_amount, " +
						"valid_bet_amount => @valid_bet_amount, " +
						"settle_amount => @settle_amount, " +
						"commission => @commission, " +
						"turnover => @turnover, " +
						"payout => @payout, " +
						"tie_bet_adjustment => @tie_bet_adjustment, " +
						"client_status => @client_status, " +
						"reopen_turnover_adjustment => @reopen_turnover_adjustment, " +
						"reopened_on => @reopened_on, " +
						"effective_betting_percentage => @effective_betting_percentage, " +
						"actiontype => @actiontype);";

			await db.QueryFirstAsync<DbExcutionResult>(sqlQuery, parameters);
		}
	}
}