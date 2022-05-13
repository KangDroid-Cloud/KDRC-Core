using KDRC_Core.Models;
using KDRC_Core.Models.Requests;
using KDRC_Core.Repositories;

namespace KDRC_Core.Services;

public class AccountService
{
    private readonly ICommonMongoRepository<Account> _accountRepository;

    public AccountService(ICommonMongoRepository<Account> repository)
    {
        _accountRepository = repository;
    }

    public async Task<Account?> CreateAccountAsync(AccountRegisterRequest registerRequest)
    {
        var previousData = await _accountRepository.FindFirstOrDefaultAsync(a => a.UserEmail == registerRequest.Email);
        if (previousData != null) return null;

        // Create new-fresh data.
        var account = registerRequest.ToAccount();

        var createdResult = await _accountRepository.CreateOneAsync(account);
        return createdResult;
    }
}