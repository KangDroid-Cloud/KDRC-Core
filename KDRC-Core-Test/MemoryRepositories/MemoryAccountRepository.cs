using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KDRC_Core.Models;
using KDRC_Core.Repositories;

namespace KDRC_Core_Test.MemoryRepositories;

public class MemoryAccountRepository : ICommonMongoRepository<Account>
{
    private readonly List<Account> _memoryStorage;
    private readonly Func<Account, string, bool> _keyChecker = (account, s) => account.Id == s;

    public MemoryAccountRepository()
    {
        _memoryStorage = new List<Account>();
    }

    public async Task<Account?> FindSingleOrDefaultAsync(Expression<Func<Account, bool>> predicate)
    {
        var funcExpression = predicate.Compile();
        return _memoryStorage.Where(funcExpression)
            .SingleOrDefault();
    }

    public async Task<Account?> FindFirstOrDefaultAsync(Expression<Func<Account, bool>> predicate)
    {
        var funcExpression = predicate.Compile();
        return _memoryStorage.Where(funcExpression)
            .FirstOrDefault();
    }

    public async Task<List<Account>> FindMultipleAsync(Expression<Func<Account, bool>> predicate)
    {
        var funcExpression = predicate.Compile();
        return _memoryStorage.Where(funcExpression)
            .ToList();
    }

    public async Task<Account> CreateOneAsync(Account targetAccount)
    {
        if (_memoryStorage.Any(a => _keyChecker(a, targetAccount.Id)))
        {
            throw new Exception("MemoryException: Duplicated Key");
        }

        _memoryStorage.Add(targetAccount);
        return targetAccount;
    }

    public async Task<Account> UpdateOneAsync(Account targetAccount)
    {
        _memoryStorage.Remove(_memoryStorage.First(a => a.Id == targetAccount.Id));
        _memoryStorage.Add(targetAccount);
        return targetAccount;
    }

    public async Task DeleteOneAsync(Account targetT)
    {
        _memoryStorage.Remove(_memoryStorage.First(a => a.Id == targetT.Id));
    }
}