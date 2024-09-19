using MongoDB.Bson;
using MongoDB.Driver;
using SabaRecord.Core.Repositories;
using SabaRecord.Repositories.Models;

namespace SabaRecord.Repositories;

internal class ApiLogsRepository(IMongoClient client, ISentryClient sentryClient) : IApiLogsRepository
{
	public async ValueTask<bool> AddAsync(string gameProviderCode, string action, string reqJson, string resJson, CancellationToken cancellationToken = default)
	{
		try
		{
			var collection = GetCollection(gameProviderCode, action);
			var document = new ApiCallLog()
			{
				Time = DateTime.UtcNow,
				Request = BsonDocument.Parse(reqJson),
				Response = BsonDocument.Parse(resJson)
			};

			await collection.InsertOneAsync(
				document: document,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return true;
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);
			return true;
		}
	}

	private IMongoCollection<ApiCallLog> GetCollection(string gameProviderCode, string action)
	=> client
	.GetDatabase($"{gameProviderCode.ToLower()}_logs_master")
	.GetCollection<ApiCallLog>($"{action.ToLower()}_logs");
}