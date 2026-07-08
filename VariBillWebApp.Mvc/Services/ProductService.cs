using VariBillWebApp.Mvc.Models.DTO;
using VariBillWebApp.Mvc.Models.ViewModels;
using VariBillWebApp.Mvc.Services.Abstractions;

namespace VariBillWebApp.Mvc.Services;

/// <summary>
/// Product service implementation.
/// </summary>
public sealed class ProductService : VariBillDomainServiceBase, IProductService
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(IVariBillApiClient apiClient, ILogger<ProductService> logger)
        : base(apiClient)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProductViewModel>> GetAllProductsAsync()
    {
        var response = await GetAsync<List<ProductApiResponseDto>>("api/products");
        return response?.Select(MapToViewModel).ToList() ?? new List<ProductViewModel>();
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(Guid id)
    {
        var dto = await GetAsync<ProductApiResponseDto>($"api/products/{id}");
        return dto is null ? null : MapToViewModel(dto);
    }

    public async Task<ProductViewModel?> CreateProductAsync(ProductFormViewModel model)
    {
        var dto = new ProductApiWriteDto(model.Name, model.Price, model.Description, model.ProductTypeId);
        return await PostAsync<ProductApiWriteDto, ProductViewModel>("api/products", dto);
    }

    public async Task<bool> UpdateProductAsync(Guid id, ProductEditViewModel model)
    {
        var dto = new ProductApiWriteDto(model.Name, model.Price, model.Description, model.ProductTypeId);
        var response = await PutAsync($"api/products/{id}", dto);
        if (!response.IsSuccessStatusCode)
            _logger.LogWarning("Failed to update product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var response = await DeleteAsync($"api/products/{id}");
        if (!response.IsSuccessStatusCode)
            _logger.LogWarning("Failed to delete product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
        return response.IsSuccessStatusCode;
    }
    private static ProductViewModel MapToViewModel(ProductApiResponseDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            SKU = dto.SKU,
            Price = dto.Price,
            Quantity = dto.Quantity,
            Description = dto.Description,
            ProductTypeId = dto.ProductTypeId,
            ProductTypeName = dto.ProductTypeName,
            IsActive = dto.IsActive,
            CreatedDate = dto.DateCreated,
            ModifiedDate = dto.DateModified
        };
}




