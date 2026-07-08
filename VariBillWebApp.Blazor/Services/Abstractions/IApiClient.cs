namespace VariBillWebApp.Blazor.Services.Abstractions;

/// <summary>
/// Abstraction for authenticated HTTP calls to the backend API.
/// </summary>
public interface IApiClient
{
    /// <summary>Performs a GET request and returns the raw response.</summary>
    Task<HttpResponseMessage> GetAsync(string endpoint);

    /// <summary>Performs a GET request and deserializes the response.</summary>
    Task<TResponse?> GetAsync<TResponse>(string endpoint);

    /// <summary>Performs a POST request with JSON body and returns raw response.</summary>
    Task<HttpResponseMessage> PostAsync<TRequest>(string endpoint, TRequest requestBody);

    /// <summary>Performs a POST request and deserializes the response.</summary>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestBody);

    /// <summary>Performs a PUT request with JSON body and returns raw response.</summary>
    Task<HttpResponseMessage> PutAsync<TRequest>(string endpoint, TRequest requestBody);

    /// <summary>Performs a PUT request and deserializes the response.</summary>
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest requestBody);

    /// <summary>Performs a DELETE request.</summary>
    Task<HttpResponseMessage> DeleteAsync(string endpoint);
}
