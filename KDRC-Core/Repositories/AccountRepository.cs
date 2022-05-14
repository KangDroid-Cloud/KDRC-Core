using System.Linq.Expressions;
using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using KDRC_Core.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace KDRC_Core.Repositories;

public class AccountRepository : ICommonMongoRepository<Account>
{
    private readonly IMongoCollection<Account> _accountCollection;
    private IMongoQueryable<Account> Accounts => _accountCollection.AsQueryable();

    public AccountRepository(MongoContext mongoContext)
    {
        _accountCollection = mongoContext.Accounts;
    }

    public async Task<Account?> FindSingleOrDefaultAsync(Expression<Func<Account, bool>> predicate)
    {
        return await Accounts.Where(predicate)
            .SingleOrDefaultAsync();
    }

    public async Task<Account?> FindFirstOrDefaultAsync(Expression<Func<Account, bool>> predicate)
    {
        return await Accounts.Where(predicate)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Account>> FindMultipleAsync(Expression<Func<Account, bool>> predicate)
    {
        return await Accounts.Where(predicate)
            .ToListAsync();
    }

    public async Task<Account> CreateOneAsync(Account targetAccount)
    {
        await _accountCollection.InsertOneAsync(targetAccount);
        return targetAccount;
    }

    public async Task<Account> UpdateOneAsync(Account targetAccount)
    {
        await _accountCollection.ReplaceOneAsync(a => a.Id == targetAccount.Id, targetAccount,
            new ReplaceOptions
            {
                IsUpsert = true
            });

        return targetAccount;
    }

    public async Task DeleteOneAsync(Account targetAccount)
    {
        await _accountCollection.DeleteOneAsync(a => a.Id == targetAccount.Id);
    }
}