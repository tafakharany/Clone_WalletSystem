namespace Wallet.API.Tests.Integration;

public class TransferAmountToOtherWalletAccountTests : IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly WebApplicationFactory<IApiMarker> _webApplicationFactory;
    private string? _token;
    public TransferAmountToOtherWalletAccountTests(WebApplicationFactory<IApiMarker> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task GivenValidTransactionToSend_ButUnauthorized()
    {
        //Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var request = new TransferRequestDto
        {
            Amount = 100,
            RecipientMobileNumber = "01102720157",
        };
        //Act
        var response = await httpClient.PostAsJsonAsync("WalletProcesses/SendMoney", request);
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenValidTransactionToSend_ButAuthorized_NotExistingRecipient()
    {
        //Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var loginRequest = new LoginRequestDto()
        {
            MobileNumber = "01093040056",
            Password = "P@ssw0rd"
        };
        _token = await GetToken(loginRequest);
        var request = new TransferRequestDto
        {
            Amount = 100,
            RecipientMobileNumber = "01102720157"
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);
        //Act
        var response = await httpClient.PostAsJsonAsync("WalletProcesses/SendMoney", request);
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResponseDto>();
        result?.ResponseCode.Should().Be(ResponseCodes.FailedToProcess);
        result?.ResponseMessage.Should().Be(Resource.RecipientMobileISNotFound);
    }

    [Fact]
    public async Task GivenValidTransactionToSend_ButAuthorized_ExistingRecipient()
    {
        //Arrange
        var httpClient = _webApplicationFactory.CreateClient();
        var loginRequest = new LoginRequestDto()
        {
            MobileNumber = "01093040056",
            Password = "P@ssw0rd"
        };
        _token = await GetToken(loginRequest);

        var request = new TransferRequestDto
        {
            Amount = 100,
            RecipientMobileNumber = "01018565553"
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);
        //Act
        var response = await httpClient.PostAsJsonAsync("WalletProcesses/SendMoney", request);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ResponseDto>();
        result?.ResponseCode.Should().Be(ResponseCodes.ProcessedSuccessfully);
        result?.ResponseMessage.Should().Be(Resource.Sucess);
    }


    #region Utils

    private async Task<string?> GetToken(LoginRequestDto request)
    {
        var httpClient = _webApplicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("Identity/login", request);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        var token = result?.AccessToken;
        return token;
    }


    #endregion
}