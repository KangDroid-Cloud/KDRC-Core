using KDRC_Core.Extensions;
using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using KDRC_Core.Models.Requests;
using KDRC_Core.Repositories;

namespace KDRC_Core.Services;

public class AuthService
{
    private readonly ICommonMongoRepository<Account> _accountRepository;
    private readonly ICommonMongoRepository<AccessToken> _accessTokenRepository;

    public AuthService(ICommonMongoRepository<Account> accountRepository,
        ICommonMongoRepository<AccessToken> repository)
    {
        _accountRepository = accountRepository;
        _accessTokenRepository = repository;
    }

    public async Task<AccessToken?> LoginAccountAsync(LoginRequest loginRequest)
    {
        // Find Account exists.
        var account = await _accountRepository.FindFirstOrDefaultAsync(a => a.UserEmail == loginRequest.Email);

        // If account is null -> meaning no user exists.
        if (account == null) return null;

        // Verify Hashed Password -> false -> meaning password is wrong.
        if (!account.UserPassword.CheckHashedPassword(loginRequest.Password)) return null;

        // Create Access Token -> Save
        var accessToken = AccessToken.CreateDefault(account.Id);
        await _accessTokenRepository.CreateOneAsync(accessToken);

        return accessToken;
    }
}