using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KDRC_Core_Test.Helpers;
using KDRC_Core.Models;
using KDRC_Core.Repositories;
using KDRC_Core.Services;
using Xunit;

namespace KDRC_Core_Test.Repositories;

// Do we really need this?
[Collection("IntegrationCollections")]
public class AccountRepositoryTest
{
    private readonly MongoContext _mongoContext;
    private readonly ICommonMongoRepository<Account> _accountRepository;

    public AccountRepositoryTest(IntegrationTestFixture fixture)
    {
        _mongoContext = fixture._MongoContext;
        _accountRepository = new AccountRepository(_mongoContext);
    }

    [Fact(DisplayName =
        "FindSingleOrDefaultAsync: FindSingleOrDefaultAsync should return null if data does not exists.")]
    public async Task Is_FindSingleOrDefaultAsync_Returns_Null_When_Data_Not_Exists()
    {
        // Do
        var result = await _accountRepository.FindSingleOrDefaultAsync(a => a.Id == "test");

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindSingleOrDefaultAsync: FindSingleOrDefaultAsync should return single data if exists.")]
    public async Task Is_FindSingleOrDefaultAsync_Returns_Data_If_Exists()
    {
        // Let
        var account = new Account
        {
            Id = Ulid.NewUlid().ToString()
        };
        await _accountRepository.CreateOneAsync(account);

        // Do
        var result = await _accountRepository.FindSingleOrDefaultAsync(a => a.Id == account.Id);

        // Check
        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
    }

    [Fact(DisplayName =
        "FindFirstOrDefaultAsync: FindSingleOrDefaultAsync should return null if data does not exists.")]
    public async Task Is_FindFirstOrDefaultAsync_Returns_Null_When_Data_Not_Exists()
    {
        // Do
        var result = await _accountRepository.FindFirstOrDefaultAsync(a => a.Id == "test");

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName = "FindFirstOrDefaultAsync: FindSingleOrDefaultAsync should return single data if exists.")]
    public async Task Is_FindFirstOrDefaultAsync_Returns_Data_If_Exists()
    {
        // Let
        var account = new Account
        {
            Id = Ulid.NewUlid().ToString()
        };
        await _accountRepository.CreateOneAsync(account);

        // Do
        var result = await _accountRepository.FindFirstOrDefaultAsync(a => a.Id == account.Id);

        // Check
        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
    }

    [Fact(DisplayName = "FindMultipleAsync: FindMultipleAsync should return corresponding data list if exists.")]
    public async Task Is_FindMultipleAsync_Returns_Corresponding_Lists()
    {
        // Let
        var account = new Account
        {
            Id = Ulid.NewUlid().ToString()
        };
        await _accountRepository.CreateOneAsync(account);

        // Do
        var result = await _accountRepository.FindMultipleAsync(a => a.Id == account.Id);

        // Check
        Assert.Single(result);
    }

    [Fact(DisplayName = "CreateOneAsync: CreateOneAsync should create one data properly")]
    public async Task Is_CreateOneAsync_Creates_Data_Properly()
    {
        // Let
        var account = new Account
        {
            Id = Ulid.NewUlid().ToString()
        };

        // Do
        await _accountRepository.CreateOneAsync(account);

        // Check
        var result = await _accountRepository.FindSingleOrDefaultAsync(a => a.Id == account.Id);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "UpdateOneAsync: UpdateOneAsync should update corresponding data properly")]
    public async Task Is_UpdateOneAsync_Updates_Data_Properly()
    {
        // Let
        var account = new Account
        {
            Id = Ulid.NewUlid().ToString()
        };
        await _accountRepository.CreateOneAsync(account);
        var toChangeAccount = new Account
        {
            Id = account.Id,
            AccountRoles = new HashSet<AccountRole>
            {
                AccountRole.Admin
            }
        };

        // Do
        await _accountRepository.UpdateOneAsync(toChangeAccount);

        // Check
        var response = await _accountRepository.FindFirstOrDefaultAsync(a => a.Id == toChangeAccount.Id);
        Assert.NotNull(response);
        Assert.Single(response.AccountRoles);
        Assert.Contains(AccountRole.Admin, response.AccountRoles);
    }

    [Fact(DisplayName = "DeleteOneAsync: DeleteOneAsync should remove corresponding data correctly")]
    public async Task Is_DeleteOneAsync_Removes_Data_Correctly()
    {
        // Let
        var account = new Account
        {
            Id = Ulid.NewUlid().ToString()
        };
        await _accountRepository.CreateOneAsync(account);

        // Do
        await _accountRepository.DeleteOneAsync(account);

        // Check
        Assert.Empty(await _accountRepository.FindMultipleAsync(a => a.Id == account.Id));
    }
}