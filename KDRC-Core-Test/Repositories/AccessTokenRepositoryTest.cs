using System;
using System.Threading.Tasks;
using KDRC_Core_Test.Helpers;
using KDRC_Core.Models.Data;
using KDRC_Core.Repositories;
using KDRC_Core.Services;
using Xunit;

namespace KDRC_Core_Test.Repositories;

[Collection("IntegrationCollections")]
public class AccessTokenRepositoryTest
{
    private readonly MongoContext _mongoContext;
    private readonly ICommonMongoRepository<AccessToken> _accessTokenRepository;

    public AccessTokenRepositoryTest(IntegrationTestFixture fixture)
    {
        _mongoContext = fixture._MongoContext;
        _accessTokenRepository = new AccessTokenRepository(_mongoContext);
    }

    [Fact(DisplayName =
        "FindSingleOrDefaultAsync: FindSingleOrDefaultAsync should return null if data does not exists.")]
    public async Task Is_FindSingleOrDefaultAsync_Returns_Null_When_Data_Not_Exists()
    {
        // Do
        var result = await _accessTokenRepository.FindSingleOrDefaultAsync(a => a.Id == "test");

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindSingleOrDefaultAsync: FindSingleOrDefaultAsync should return single data if exists.")]
    public async Task Is_FindSingleOrDefaultAsync_Returns_Data_If_Exists()
    {
        // Let
        var accessToken = AccessToken.CreateDefault(Ulid.NewUlid().ToString());
        await _accessTokenRepository.CreateOneAsync(accessToken);

        // Do
        var result = await _accessTokenRepository.FindSingleOrDefaultAsync(a => a.Id == accessToken.Id);

        // Check
        Assert.NotNull(result);
        Assert.Equal(accessToken.Id, result.Id);
    }

    [Fact(DisplayName =
        "FindFirstOrDefaultAsync: FindSingleOrDefaultAsync should return null if data does not exists.")]
    public async Task Is_FindFirstOrDefaultAsync_Returns_Null_When_Data_Not_Exists()
    {
        // Do
        var result = await _accessTokenRepository.FindFirstOrDefaultAsync(a => a.Id == "test");

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindFirstOrDefaultAsync: FindSingleOrDefaultAsync should return single data if exists.")]
    public async Task Is_FindFirstOrDefaultAsync_Returns_Data_If_Exists()
    {
        // Let
        var accessToken = AccessToken.CreateDefault(Ulid.NewUlid().ToString());
        await _accessTokenRepository.CreateOneAsync(accessToken);

        // Do
        var result = await _accessTokenRepository.FindFirstOrDefaultAsync(a => a.Id == accessToken.Id);

        // Check
        Assert.NotNull(result);
        Assert.Equal(accessToken.Id, result.Id);
    }

    [Fact(DisplayName = "FindMultipleAsync: FindMultipleAsync should return corresponding data list if exists.")]
    public async Task Is_FindMultipleAsync_Returns_Corresponding_Lists()
    {
        // Let
        var accessToken = AccessToken.CreateDefault(Ulid.NewUlid().ToString());
        await _accessTokenRepository.CreateOneAsync(accessToken);

        // Do
        var result = await _accessTokenRepository.FindMultipleAsync(a => a.Id == accessToken.Id);

        // Check
        Assert.Single(result);
    }

    [Fact(DisplayName = "CreateOneAsync: CreateOneAsync should create one data properly")]
    public async Task Is_CreateOneAsync_Creates_Data_Properly()
    {
        // Let
        var accessToken = AccessToken.CreateDefault(Ulid.NewUlid().ToString());

        // Do
        await _accessTokenRepository.CreateOneAsync(accessToken);

        // Check
        var result = await _accessTokenRepository.FindSingleOrDefaultAsync(a => a.Id == accessToken.Id);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "UpdateOneAsync: UpdateOneAsync should update corresponding data properly")]
    public async Task Is_UpdateOneAsync_Updates_Data_Properly()
    {
        // Let
        var accessToken = AccessToken.CreateDefault(Ulid.NewUlid().ToString());
        await _accessTokenRepository.CreateOneAsync(accessToken);
        var toChangeToken = new AccessToken
        {
            Id = accessToken.Id,
            ValidUntil = default,
            UserId = Ulid.NewUlid().ToString()
        };

        // Do
        await _accessTokenRepository.UpdateOneAsync(toChangeToken);

        // Get one
        var changed = await _accessTokenRepository.FindFirstOrDefaultAsync(a => a.Id == toChangeToken.Id);
        Assert.NotNull(changed);
        Assert.Equal(toChangeToken.Id, changed.Id);
        Assert.Equal(toChangeToken.ValidUntil, changed.ValidUntil);
        Assert.Equal(toChangeToken.UserId, changed.UserId);
    }

    [Fact(DisplayName = "DeleteOneAsync: DeleteOneAsync should remove corresponding data correctly")]
    public async Task Is_DeleteOneAsync_Removes_Data_Correctly()
    {
        // Let
        var accessToken = AccessToken.CreateDefault(Ulid.NewUlid().ToString());
        await _accessTokenRepository.CreateOneAsync(accessToken);

        // Do
        await _accessTokenRepository.DeleteOneAsync(accessToken);

        // Check
        Assert.Empty(await _accessTokenRepository.FindMultipleAsync(a => a.Id == accessToken.Id));
    }
}