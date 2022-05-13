using System.Threading.Tasks;
using KDRC_Core_Test.MemoryRepositories;
using KDRC_Core.Models;
using KDRC_Core.Models.Requests;
using KDRC_Core.Repositories;
using KDRC_Core.Services;
using Xunit;

namespace KDRC_Core_Test.Services;

public class AccountServiceTest
{
    private readonly ICommonMongoRepository<Account> _memoryAccountRepository;
    private readonly AccountService _accountService;

    public AccountServiceTest()
    {
        _memoryAccountRepository = new MemoryAccountRepository();
        _accountService = new AccountService(_memoryAccountRepository);
    }

    [Fact(DisplayName =
        "CreateAccountAsync: CreateAccountAsync should return null if corresponding email already exists.")]
    public async Task Is_CreateAccountAsync_Returns_Null_When_Email_Already_Exists()
    {
        // Let
        var request = new AccountRegisterRequest
        {
            Email = "test@test.com",
            NickName = "TestNickName",
            Password = "hello"
        };
        await _memoryAccountRepository.CreateOneAsync(request.ToAccount());

        // Do
        var result = await _accountService.CreateAccountAsync(request);

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName =
        "CreateAccountAsync: CreateAccountAsync should register account successfully when no conflict detected.")]
    public async Task Is_CreateAccountAsync_Registers_Account_When_No_Conflict()
    {
        // Let
        var request = new AccountRegisterRequest
        {
            Email = "test@test.com",
            NickName = "TestNickName",
            Password = "hello"
        };

        // Do
        var result = await _accountService.CreateAccountAsync(request);

        // Check
        Assert.NotNull(result);
        Assert.NotNull(result.Id);
        Assert.Equal(request.Email, result.UserEmail);
        Assert.Equal(request.NickName, result.UserNickName);
        Assert.Equal(AccountState.Created, result.AccountState);
        Assert.Empty(result.JwtMap);
        Assert.Single(result.AccountRoles);
        Assert.Contains(AccountRole.User, result.AccountRoles);
        Assert.DoesNotContain(AccountRole.Admin, result.AccountRoles);
    }
}