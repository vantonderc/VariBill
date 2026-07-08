using System.Net.Http;
using VariBillWebApp.Mvc.Services.Abstractions;

namespace VariBillWebApp.Mvc.Services;

public abstract class VariBillDomainServiceBase : IVariBillApiOperations
{
    private readonly IVariBillApiClient _apiClient;

    protected VariBillDomainServiceBase(IVariBillApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<HttpResponseMessage> GetAsync(string endpoint) =>
        _apiClient.GetAsync(endpoint);

    public Task<TResponse?> GetAsync<TResponse>(string endpoint) =>
        _apiClient.GetAsync<TResponse>(endpoint);

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestBody, string? correlationId = null) =>
        _apiClient.PostAsync<TRequest, TResponse>(endpoint, requestBody, correlationId);

    public Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content) =>
        _apiClient.PostMultipartAsync<TResponse>(endpoint, content);

    public Task<HttpResponseMessage> PostMultipartRawAsync(string endpoint, MultipartFormDataContent content) =>
        _apiClient.PostMultipartRawAsync(endpoint, content);

    public Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest body) =>
        _apiClient.PostRawAsync(endpoint, body);

    public Task<HttpResponseMessage> PostRawAsync(string endpoint, HttpContent content) =>
        _apiClient.PostRawAsync(endpoint, content);

    public Task<HttpResponseMessage> PutAsync<TRequest>(string endpoint, TRequest body) =>
        _apiClient.PutAsync(endpoint, body);

    public Task<HttpResponseMessage> DeleteAsync(string endpoint) =>
        _apiClient.DeleteAsync(endpoint);
}


