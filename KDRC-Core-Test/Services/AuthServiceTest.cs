using System;
using System.Threading.Tasks;
using KDRC_Core_Test.MemoryRepositories;
using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using KDRC_Core.Models.Requests;
using KDRC_Core.Repositories;
using KDRC_Core.Services;
using Xunit;

namespace KDRC_Core_Test.Services;

public class AuthServiceTest
{
    private readonly AuthService _authService;
    private readonly ICommonMongoRepository<AccessToken> _memoryAccessTokenRepository;
    private readonly ICommonMongoRepository<Account> _memoryAccountRepository;

    public AuthServiceTest()
    {
        _memoryAccountRepository = new MemoryAccountRepository();
        _memoryAccessTokenRepository = new MemoryAccessTokenRepository();

        _authService = new AuthService(_memoryAccountRepository, _memoryAccessTokenRepository);
    }

    [Fact(DisplayName =
        "LoginAccountAsync: LoginAccountAsync should return null when corresponding email does not exists.")]
    public async Task Is_LoginAccountAsync_Returns_Null_When_No_Email_Found()
    {
        // Let
        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "testasdf"
        };

        // Do
        var result = await _authService.LoginAccountAsync(loginRequest);

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName = "LoginAccountAsync: LoginAccountAsync should return null when password is differ.")]
    public async Task Is_LoginAccountAsync_Returns_Null_Password_Differ()
    {
        // Let
        var registerRequest = new AccountRegisterRequest
        {
            Email = "test@test.com",
            Password = "testasdf",
            NickName = "asdfasdf"
        };
        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = "anotherwrongpassword"
        };
        await _memoryAccountRepository.CreateOneAsync(registerRequest.ToAccount());

        // Do
        var result = await _authService.LoginAccountAsync(loginRequest);

        // Check
        Assert.Null(result);
    }

    [Fact(DisplayName =
        "LoginAccountAsync: LoginAccountAsync should properly return access token when successfully logged-in.")]
    public async Task Is_LoginAccountAsync_Returns_AccessToken_When_Login_Success()
    {
        // Let
        var registerRequest = new AccountRegisterRequest
        {
            Email = "test@test.com",
            Password = "testasdf",
            NickName = "asdfasdf"
        };
        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };
        var account = registerRequest.ToAccount();
        await _memoryAccountRepository.CreateOneAsync(account);

        // Do
        var result = await _authService.LoginAccountAsync(loginRequest);

        // Check Response
        Assert.NotNull(result);
        Assert.NotEqual(default(DateTimeOffset), result.ValidUntil);
        Assert.Equal(account.Id, result.UserId);
        Assert.NotEmpty(result.Id);

        // Check Actually saved
        var accessToken = await _memoryAccessTokenRepository.FindFirstOrDefaultAsync(a => a.Id == result.Id);
        Assert.NotNull(accessToken);
        Assert.Equal(accessToken.Id, result.Id);
        Assert.Equal(accessToken.UserId, result.UserId);
        Assert.Equal(accessToken.ValidUntil, result.ValidUntil);
    }
}