using IdentityModel.Client;
using Microsoft.Extensions.Options;
using VariBillWebApp.Mvc.Settings;

namespace VariBillWebApp.Mvc.Utils;

/// <summary>
/// Helper to obtain a client‑credentials token from IdentityServer.
/// </summary>
public class APIValidation
{
    private readonly APISettings _apiSettings;

    public APIValidation(IOptions<APISettings> apiSettings)
    {
        _apiSettings = apiSettings.Value;
    }

    /// <summary>
    /// Requests a machine‑to‑machine access token.
    /// </summary>
    public async Task<APIAccessTokenResponseDto> CheckServerAsync(HttpClient client)
    {
        var response = new APIAccessTokenResponseDto { Success = false, Message = "Token Fail" };

        var disco = await client.GetDiscoveryDocumentAsync(_apiSettings.IdentityServer);
        if (disco.IsError)
        {
            response.Message = disco.Error;
            return response;
        }

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _apiSettings.APIClient,
            ClientSecret = _apiSettings.APISecret,
            Scope = _apiSettings.APIScope
        });

        if (tokenResponse.IsError)
        {
            response.Message = tokenResponse.Error;
            return response;
        }

        response.Token = tokenResponse.AccessToken;
        response.Success = true;
        response.Message = "Token Success";
        return response;
    }
}

/// <summary>
/// Token response DTO.
/// </summary>
public class APIAccessTokenResponseDto
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}





