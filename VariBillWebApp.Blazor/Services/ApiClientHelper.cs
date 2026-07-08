using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using VariBillWebApp.Blazor.Services.Abstractions;
using VariBillWebApp.Blazor.Settings;
using VariBillWebApp.Blazor.Utils;

namespace VariBillWebApp.Blazor.Services;

/// <summary>
/// Authenticated HTTP client that forwards the user's token or falls back to client credentials.
/// </summary>
public class ApiClientHelper : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiClientHelper> _logger;
    private readonly APIValidation _apiValidation;

    public ApiClientHelper(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ApiClientHelper> logger,
        APIValidation apiValidation)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _apiValidation = apiValidation;
    }

    private async Task<HttpClient?> CreateAuthenticatedClientAsync()
    {
        var client = _httpClientFactory.CreateClient("VariBillApi");
        var httpContext = _httpContextAccessor.HttpContext;
        // 1. Try to use the signed‑in user's token
        try
        {
            if (httpContext?.User.Identity?.IsAuthenticated == true)
            {
                var token = await httpContext.GetTokenAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                {
                    client.SetBearerToken(token);
                    _logger.LogDebug("Blazor.ApiClientHelper: using user access token");
                    return client;
                }
                _logger.LogDebug("Blazor.ApiClientHelper: no user access token available");
            }
            else
            {
                _logger.LogDebug("Blazor.ApiClientHelper: no authenticated user in HttpContext");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blazor.ApiClientHelper: error reading user token");
        }

        // 2. Fallback to client credentials
        try
        {
            var tokenResponse = await _apiValidation.CheckServerAsync(client);
            if (tokenResponse?.Success == true && !string.IsNullOrEmpty(tokenResponse.Token))
            {
                client.SetBearerToken(tokenResponse.Token);
                _logger.LogDebug("Blazor.ApiClientHelper: using client_credentials token");
                return client;
            }
            _logger.LogWarning("Blazor.ApiClientHelper: client_credentials failed: {Message}", tokenResponse?.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blazor.ApiClientHelper: exception requesting client credentials token");
        }

        _logger.LogWarning("Blazor.ApiClientHelper: Unable to obtain a valid access token.");
        return null;
    }

    private static StringContent ToJsonContent<T>(T body) =>
        new(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        var client = await CreateAuthenticatedClientAsync();
        if (client is null)
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        return await client.GetAsync(endpoint);
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint)
    {
        var response = await GetAsync(endpoint);
        return await DeserializeSuccessResponseAsync<TResponse>(response);
    }

    public async Task<HttpResponseMessage> PostAsync<TRequest>(string endpoint, TRequest requestBody)
    {
        var client = await CreateAuthenticatedClientAsync();
        if (client is null)
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        return await client.PostAsync(endpoint, ToJsonContent(requestBody));
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestBody)
    {
        var response = await PostAsync(endpoint, requestBody);
        return await DeserializeSuccessResponseAsync<TResponse>(response);
    }

    public async Task<HttpResponseMessage> PutAsync<TRequest>(string endpoint, TRequest requestBody)
    {
        var client = await CreateAuthenticatedClientAsync();
        if (client is null)
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        return await client.PutAsync(endpoint, ToJsonContent(requestBody));
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest requestBody)
    {
        var response = await PutAsync(endpoint, requestBody);
        return await DeserializeSuccessResponseAsync<TResponse>(response);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        var client = await CreateAuthenticatedClientAsync();
        if (client is null)
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        return await client.DeleteAsync(endpoint);
    }

    private async Task<TResponse?> DeserializeSuccessResponseAsync<TResponse>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("API call failed. Status: {StatusCode}, Error: {Error}", response.StatusCode, error);
            return default;
        }

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize API response.");
            throw;
        }
    }
}
