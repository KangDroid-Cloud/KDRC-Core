using System;
using KDRC_Core.Configurations;
using KDRC_Core.Services;
using MongoDB.Driver;
using Xunit;

namespace KDRC_Core_Test.Helpers;

[CollectionDefinition("IntegrationCollections")]
public class IntegrationTestDefinition : ICollectionFixture<IntegrationTestFixture>
{
}

public class IntegrationTestFixture
{
    private readonly MongoClient _MongoClient;
    public MongoContext _MongoContext => new(TestMongoConfiguration);

    public MongoConfiguration TestMongoConfiguration => new MongoConfiguration
    {
        ConnectionString = ConnectionString,
        DatabaseName = "IntegrationTestDatabase",
        AccountCollectionName = Ulid.NewUlid().ToString(),
        AccessTokenCollectionName = Ulid.NewUlid().ToString()
    };

    private const string ConnectionString = "mongodb://root:testPassword@localhost:27017";

    public IntegrationTestFixture()
    {
        _MongoClient = new MongoClient(ConnectionString);
    }
}