using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KDRC_Core.Models.Data;
using KDRC_Core.Repositories;

namespace KDRC_Core_Test.MemoryRepositories;

public class MemoryAccessTokenRepository : ICommonMongoRepository<AccessToken>
{
    private readonly List<AccessToken> _memoryStorage;
    private readonly Func<AccessToken, string, bool> _keyChecker = (account, s) => account.Id == s;

    public MemoryAccessTokenRepository()
    {
        _memoryStorage = new List<AccessToken>();
    }

    public async Task<AccessToken?> FindSingleOrDefaultAsync(Expression<Func<AccessToken, bool>> predicate)
    {
        var funcExpression = predicate.Compile();
        return _memoryStorage.Where(funcExpression)
            .SingleOrDefault();
    }

    public async Task<AccessToken?> FindFirstOrDefaultAsync(Expression<Func<AccessToken, bool>> predicate)
    {
        var funcExpression = predicate.Compile();
        return _memoryStorage.Where(funcExpression)
            .FirstOrDefault();
    }

    public async Task<List<AccessToken>> FindMultipleAsync(Expression<Func<AccessToken, bool>> predicate)
    {
        var funcExpression = predicate.Compile();
        return _memoryStorage.Where(funcExpression)
            .ToList();
    }

    public async Task<AccessToken> CreateOneAsync(AccessToken targetAccessToken)
    {
        if (_memoryStorage.Any(a => _keyChecker(a, targetAccessToken.Id)))
        {
            throw new Exception("MemoryException: Duplicated Key");
        }

        _memoryStorage.Add(targetAccessToken);
        return targetAccessToken;
    }

    public async Task<AccessToken> UpdateOneAsync(AccessToken targetAccessToken)
    {
        _memoryStorage.Remove(_memoryStorage.First(a => a.Id == targetAccessToken.Id));
        _memoryStorage.Add(targetAccessToken);
        return targetAccessToken;
    }

    public async Task DeleteOneAsync(AccessToken targetT)
    {
        _memoryStorage.Remove(_memoryStorage.First(a => a.Id == targetT.Id));
    }
}