using KDRC_Core.Configurations;
using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using MongoDB.Driver;

namespace KDRC_Core.Services;

public class MongoContext
{
    private readonly IMongoClient _mongoClient;
    private readonly MongoConfiguration _mongoConfiguration;

    public IMongoDatabase Database => _mongoClient.GetDatabase(_mongoConfiguration.DatabaseName);

    public IMongoCollection<Account> Accounts =>
        Database.GetCollection<Account>(_mongoConfiguration.AccountCollectionName);

    public IMongoCollection<AccessToken> AccessTokens =>
        Database.GetCollection<AccessToken>(_mongoConfiguration.AccessTokenCollectionName);

    public MongoContext(MongoConfiguration mongoConfiguration)
    {
        _mongoConfiguration = mongoConfiguration;
        _mongoClient = new MongoClient(mongoConfiguration.ConnectionString);
    }
}