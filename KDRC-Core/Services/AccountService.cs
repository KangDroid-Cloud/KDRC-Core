using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using KDRC_Core.Models.Requests;
using KDRC_Core.Repositories;
using KDRC_Models.EventMessages.Account;

namespace KDRC_Core.Services;

public class AccountService
{
    private readonly ICommonMongoRepository<Account> _accountRepository;
    private readonly IEventService _eventService;

    public AccountService(ICommonMongoRepository<Account> repository, IEventService eventService)
    {
        _accountRepository = repository;
        _eventService = eventService;
    }

    public async Task<Account?> CreateAccountAsync(AccountRegisterRequest registerRequest)
    {
        var previousData = await _accountRepository.FindFirstOrDefaultAsync(a => a.UserEmail == registerRequest.Email);
        if (previousData != null) return null;

        // Create new-fresh data.
        var account = registerRequest.ToAccount();

        var createdResult = await _accountRepository.CreateOneAsync(account);

        // Publish Message
        await _eventService.PublishMessageAsync(new AccountCreatedMessage
        {
            AccountId = createdResult.Id,
            CreatedAt = DateTimeOffset.UtcNow
        });
        return createdResult;
    }
}