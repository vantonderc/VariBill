
using VariBillWebApp.Mvc.Services.Abstractions;

public interface IVariBillApiClient
{
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestBody, string? correlationId = null);
    Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content);
    Task<HttpResponseMessage> PostMultipartRawAsync(string endpoint, MultipartFormDataContent content);
    Task<HttpResponseMessage> PostRawAsync<TRequest>(string endpoint, TRequest body);
    Task<HttpResponseMessage> PostRawAsync(string endpoint, HttpContent content);
    Task<HttpResponseMessage> GetAsync(string endpoint);
    Task<TResponse?> GetAsync<TResponse>(string endpoint);
    Task<HttpResponseMessage> PutAsync<TRequest>(string endpoint, TRequest body);
    Task<HttpResponseMessage> DeleteAsync(string endpoint);
}
