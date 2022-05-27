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
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace KDRC_Core_Test.Controllers;

[Collection("IntegrationCollections")]
public class AuthControllerTest : IDisposable
{
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly HttpClient _httpClient;

    public AuthControllerTest(IntegrationTestFixture fixture)
    {
        var mongoConfig = fixture.TestMongoConfiguration;
        var dictionaryConfiguration = new Dictionary<string, string>
        {
            ["MongoSection:ConnectionString"] = mongoConfig.ConnectionString,
            ["MongoSection:DatabaseName"] = mongoConfig.DatabaseName,
            ["MongoSection:AccountCollectionName"] = mongoConfig.AccountCollectionName,
            ["MongoSection:AccessTokenCollectionName"] = mongoConfig.AccessTokenCollectionName,
            ["RabbitMq:Host"] = "localhost",
            ["RabbitMq:VirtualHost"] = "/"
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

    [Fact(DisplayName =
        "(LoginAsync) POST /api/auth/login should return unauthorized result when one of credential is not correct.")]
    public async Task Is_LoginAsync_Returns_Unauthorized_When_Credential_Wrong()
    {
        // Let
        var loginRequest = new LoginRequest
        {
            Email = "asdf@asdf.com",
            Password = "testasdf"
        };

        // Do
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Check Code
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        // Check Error Body
        var responseBody = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(responseBody);
        Assert.NotEmpty(responseBody.TraceId);
        Assert.NotEmpty(responseBody.Message);
    }

    [Fact(DisplayName =
        "(LoginAsync) POST /api/auth/login should return 200 ok with token response when login succeeds.")]
    public async Task Is_LoginAsync_Returns_Token_Response_When_Login_Succeeds()
    {
        // Let
        var registerRequest = new AccountRegisterRequest
        {
            Email = "asdgfasdfasdf@asdfasdf.com",
            Password = "asdfasdfasdfasdfasdf",
            NickName = "sadfadfsasdf"
        };
        await _httpClient.PostAsJsonAsync("/api/account/register", registerRequest);
        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        // Do
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Check Response Code
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Check Response Body
        var responseBody = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(responseBody);
        Assert.NotEmpty(responseBody.Token);
        Assert.NotEqual(default, responseBody.ValidUntil);
    }
}