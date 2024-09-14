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
	private readonly string spTemplate = "bet.bet_rounds_{0}_set";
	private readonly string _connectionString;

	public GameRoundRepository(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("Tgs")!;
	}

	public async Task ConfirmBetAsync(GameRoundSetCriteria criteria)
	{
		var parameters = new DynamicParameters();

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

		await CallBetRoundSetSPAsync(parameters, criteria.GameProviderCode);
	}

	public async Task SettleAsync(GameRoundSetCriteria criteria)
	{
		var parameters = new DynamicParameters();

		parameters.Add("@external_round_id", criteria.ExternalRoundId);
		parameters.Add("@brand_id", criteria.BrandId);
		parameters.Add("@game_id", criteria.GameId);
		parameters.Add("@external_closed_on", criteria.ExternalClosedOn);
		parameters.Add("@status", criteria.Status);
		parameters.Add("@payout", criteria.Payout);
		parameters.Add("@actiontype", criteria.ActionType);
		parameters.Add("@extrastatus", criteria.ExtraStatus);
		parameters.Add("@refid", criteria.RefId);

		await CallBetRoundSetSPAsync(parameters, criteria.GameProviderCode);
	}

	public async Task ResettleAsync(GameRoundSetCriteria criteria)
	{
		var parameters = new DynamicParameters();

		parameters.Add("@external_round_id", criteria.ExternalRoundId);
		parameters.Add("@brand_id", criteria.BrandId);
		parameters.Add("@game_id", criteria.GameId);
		parameters.Add("@external_closed_on", criteria.ExternalClosedOn);
		parameters.Add("@status", criteria.Status);
		parameters.Add("@payout", criteria.Payout);
		parameters.Add("@actiontype", criteria.ActionType);
		parameters.Add("@extrastatus", criteria.ExtraStatus);
		parameters.Add("@refid", criteria.RefId);
		parameters.Add("@isonlywinlostdatechanged", criteria.IsOnlyWinlostDateChanged);

		await CallBetRoundSetSPAsync(parameters, criteria.GameProviderCode);
	}

	public async Task UnsettleAsync(GameRoundSetCriteria criteria)
	{
		var parameters = new DynamicParameters();

		parameters.Add("@extrastatus", criteria.ExtraStatus);
		parameters.Add("@external_round_id", criteria.ExternalRoundId);
		parameters.Add("@licenseetxid", criteria.LicenseeTxId);
		parameters.Add("@brand_id", criteria.BrandId);
		parameters.Add("@game_id", criteria.GameId);
		parameters.Add("@status", criteria.Status);
		parameters.Add("@actiontype", criteria.ActionType);

		await CallBetRoundSetSPAsync(parameters, criteria.GameProviderCode);
	}

	private async Task CallBetRoundSetSPAsync(DynamicParameters parameters, string provider)
	{
		var spName = string.Format(spTemplate, provider);
		try
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = $"CALL {spName}({string.Join(", ", parameters.ParameterNames.Select(p => $"{p} => @{p}"))});";

				await db.QueryFirstAsync<DbExcutionResult>(sqlQuery, parameters);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			SentrySdk.CaptureException(ex);
		}
	}
}