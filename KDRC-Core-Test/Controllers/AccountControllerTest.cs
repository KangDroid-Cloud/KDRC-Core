using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KDRC_Core_Test.Helpers;
using KDRC_Core.Models.Requests;
using KDRC_Core.Models.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace KDRC_Core_Test.Controllers;

[Collection("IntegrationCollections")]
public class AccountControllerTest : IDisposable
{
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly HttpClient _httpClient;

    public static TheoryData<AccountRegisterRequest> BadRegisterRequest => new TheoryData<AccountRegisterRequest>
    {
        new AccountRegisterRequest
        {
            Email = Ulid.NewUlid().ToString(),
            NickName = "asdf",
            Password = "testasdf"
        },
        new AccountRegisterRequest
        {
            Email = "test@test.com",
            NickName = "asdf",
            Password = "a"
        },
        new AccountRegisterRequest
        {
            Email = "a",
            NickName = "a",
            Password = "a"
        }
    };

    public AccountControllerTest(IntegrationTestFixture fixture)
    {
        var mongoConfig = fixture.TestMongoConfiguration;
        var dictionaryConfiguration = new Dictionary<string, string>
        {
            ["MongoSection:ConnectionString"] = mongoConfig.ConnectionString,
            ["MongoSection:DatabaseName"] = mongoConfig.DatabaseName,
            ["MongoSection:AccountCollectionName"] = mongoConfig.AccountCollectionName
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(dictionaryConfiguration)
            .Build();

        _applicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseConfiguration(configuration));
        _httpClient = _applicationFactory.CreateClient();
    }

    public void Dispose()
    {
        _applicationFactory.Dispose();
    }

    [Theory(DisplayName =
        "(CreateAccountAsync): POST /api/account/register should return 400 Bad Request when requestModel is not valid.")]
    [MemberData(nameof(BadRegisterRequest), MemberType = typeof(AccountControllerTest))]
    public async Task Is_CreateAccountAsync_Returns_BadRequest_When_Model_Not_Valid(
        AccountRegisterRequest registerRequest)
    {
        // Do
        var result = await _httpClient.PostAsJsonAsync("/api/account/register", registerRequest);

        // Check 400 BadRequest
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

        // Check Response Body
        var responseBody = JsonConvert.DeserializeObject<ErrorResponse>(await result.Content.ReadAsStringAsync());
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.TraceId);
        Assert.NotNull(responseBody.Message);
    }

    [Fact(DisplayName =
        "(CreateAccountAsync): POST /api/account/register should return 409 conflict when email already exists.")]
    public async Task Is_CreateAccountAsync_Returns_409_When_Email_Already_Exists()
    {
        // Let
        var request = new AccountRegisterRequest
        {
            Email = "test@test.com",
            NickName = "testNickName",
            Password = "asdfasdfasdf"
        };
        await _httpClient.PostAsJsonAsync("/api/account/register", request);

        // Do
        var result = await _httpClient.PostAsJsonAsync("/api/account/register", request);

        // Check
        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);

        // Check Response Body
        var responseBody = JsonConvert.DeserializeObject<ErrorResponse>(await result.Content.ReadAsStringAsync());
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.TraceId);
        Assert.NotNull(responseBody.Message);
    }

    [Fact(DisplayName = "(CreateAccountAsync): POST /api/account/register should return 200 OK when registration OK.")]
    public async Task Is_CreateAccountAsync_Returns_200_Ok_When_Ok()
    {
        // Let
        var request = new AccountRegisterRequest
        {
            Email = "test@test.com",
            NickName = "testNickName",
            Password = "asdfasdfasdf"
        };

        // Do
        var result = await _httpClient.PostAsJsonAsync("/api/account/register", request);

        // Check
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
}