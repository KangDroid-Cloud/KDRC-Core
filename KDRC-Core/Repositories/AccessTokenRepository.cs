using System.Linq.Expressions;
using KDRC_Core.Models.Data;
using KDRC_Core.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace KDRC_Core.Repositories;

public class AccessTokenRepository : ICommonMongoRepository<AccessToken>
{
    private readonly IMongoCollection<AccessToken> _accessTokenCollection;
    private IMongoQueryable<AccessToken> AccessTokens => _accessTokenCollection.AsQueryable();

    public AccessTokenRepository(MongoContext mongoContext)
    {
        _accessTokenCollection = mongoContext.AccessTokens;
    }

    public async Task<AccessToken?> FindSingleOrDefaultAsync(Expression<Func<AccessToken, bool>> predicate)
    {
        return await AccessTokens.SingleOrDefaultAsync(predicate);
    }

    public async Task<AccessToken?> FindFirstOrDefaultAsync(Expression<Func<AccessToken, bool>> predicate)
    {
        return await AccessTokens.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<AccessToken>> FindMultipleAsync(Expression<Func<AccessToken, bool>> predicate)
    {
        return await AccessTokens.Where(predicate)
            .ToListAsync();
    }

    public async Task<AccessToken> CreateOneAsync(AccessToken targetAccessToken)
    {
        await _accessTokenCollection.InsertOneAsync(targetAccessToken);

        return targetAccessToken;
    }

    public async Task<AccessToken> UpdateOneAsync(AccessToken targetAccessToken)
    {
        await _accessTokenCollection.ReplaceOneAsync(a => a.Id == targetAccessToken.Id, targetAccessToken,
            new ReplaceOptions
            {
                IsUpsert = true
            });

        return targetAccessToken;
    }

    public async Task DeleteOneAsync(AccessToken targetT)
    {
        await _accessTokenCollection.DeleteOneAsync(a => a.Id == targetT.Id);
    }
}