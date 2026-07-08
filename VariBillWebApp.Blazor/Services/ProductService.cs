using VariBillWebApp.Blazor.Models.DTOs;
using VariBillWebApp.Blazor.Models.ViewModels;
using VariBillWebApp.Blazor.Services.Abstractions;

namespace VariBillWebApp.Blazor.Services;

/// <summary>
/// Product service implementation.
/// </summary>
public class ProductService : IProductService
{
    private readonly IApiClient _apiClient;
    private const string BaseRoute = "api/products";

    /// <summary>
    /// Initializes a new instance of <see cref="ProductService"/>.
    /// </summary>
    /// <param name="apiClient">Api client for calling backend endpoints.</param>
    public ProductService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<ProductApiResponseDto>> GetAllProductsAsync()
    {
        return await _apiClient.GetAsync<List<ProductApiResponseDto>>(BaseRoute) ?? new();
    }

    public async Task<ProductApiResponseDto?> GetProductByIdAsync(Guid id)
    {
        return await _apiClient.GetAsync<ProductApiResponseDto>($"{BaseRoute}/{id}");
    }

    public async Task<ProductApiResponseDto?> CreateProductAsync(ProductFormViewModel model)
    {
        var dto = new ProductApiWriteDto(model.Name, model.Price, model.Description, model.ProductTypeId);
        return await _apiClient.PostAsync<ProductApiWriteDto, ProductApiResponseDto>(BaseRoute, dto);
    }

    public async Task<bool> UpdateProductAsync(Guid id, ProductFormViewModel model)
    {
        var dto = new ProductApiWriteDto(model.Name, model.Price, model.Description, model.ProductTypeId);
        var response = await _apiClient.PutAsync($"{BaseRoute}/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var response = await _apiClient.DeleteAsync($"{BaseRoute}/{id}");
        return response.IsSuccessStatusCode;
    }
}
