

//using Microsoft.Extensions.Options;
//using VariBillWebAPI.Services.Interfaces;
//using VariBillWebAPI.Settings;


//namespace VariBillWebAPI.Services;
////TODO:this is for externamAPI acess ie B2B
//public class TokenService : ITokenService
//{
//    private readonly APISettings _apiSettings;
//    private readonly HttpClient _httpClient;

//    public TokenService(IOptions<APISettings> apiSettings, HttpClient httpClient)
//    {
//        _apiSettings = apiSettings.Value;
//        _httpClient = httpClient;
//    }

//    public async Task<string> GetAccessTokenAsync()
//    {
//        var disco = await _httpClient.GetDiscoveryDocumentAsync(_apiSettings.IdentityServer);
//        if (disco.IsError) return null;

//        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
//        {
//            Address = disco.TokenEndpoint,
//            ClientId = _apiSettings.APIClient,
//            ClientSecret = _apiSettings.APISecret,
//            Scope = _apiSettings.APIScope
//        });

//        return tokenResponse.IsError ? null : tokenResponse.AccessToken;
//    }
//}
