using VariBillWebApp.Mvc.Models.ViewModels;
using VariBillWebApp.Mvc.Services.Abstractions;

namespace VariBillWebApp.Mvc.Services;

public sealed class ProductTypeService : VariBillDomainServiceBase, IProductTypeService
{
    private readonly ILogger<ProductTypeService> _logger;

    public ProductTypeService(IVariBillApiClient apiClient, ILogger<ProductTypeService> logger)
        : base(apiClient)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProductTypeModel>> GetAllProductTypesAsync()
    {
        _logger.LogDebug("ProductTypeService.GetAllProductTypesAsync: calling API endpoint 'api/producttypes'");
        var productTypes = await GetAsync<List<ProductTypeModel>>("api/producttypes");
        if (productTypes == null)
        {
            _logger.LogWarning("ProductTypeService.GetAllProductTypesAsync: API returned null");
            return new List<ProductTypeModel>();
        }
        _logger.LogDebug("ProductTypeService.GetAllProductTypesAsync: received {Count} types", productTypes.Count);
        return productTypes;
    }

    public async Task<ProductTypeModel?> GetProductTypeByIdAsync(Guid id)
    {
        return await GetAsync<ProductTypeModel>($"api/producttypes/{id}");
    }

    public async Task<ProductTypeModel?> CreateProductTypeAsync(CreateProductTypeModel model)
    {
        _logger.LogDebug("ProductTypeService.CreateProductTypeAsync: posting to api/producttypes with name={Name}", model?.Name);
        var result = await PostAsync<CreateProductTypeModel, ProductTypeModel>("api/producttypes", model);
        if (result == null)
            _logger.LogWarning("ProductTypeService.CreateProductTypeAsync: API returned null for create");
        else
            _logger.LogDebug("ProductTypeService.CreateProductTypeAsync: created id={Id}", result.Id);
        return result;
    }

    public async Task<bool> UpdateProductTypeAsync(Guid id, UpdateProductTypeModel model)
    {
        _logger.LogDebug("ProductTypeService.UpdateProductTypeAsync: PUT api/producttypes/{Id}", id);
        var response = await PutAsync($"api/producttypes/{id}", model);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to update product type {ProductTypeId}. Status: {StatusCode}, Error: {Error}", id, response.StatusCode, err);
        }
        else
        {
            _logger.LogDebug("ProductTypeService.UpdateProductTypeAsync: update succeeded for {Id}", id);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteProductTypeAsync(Guid id)
    {
        _logger.LogDebug("ProductTypeService.DeleteProductTypeAsync: DELETE api/producttypes/{Id}", id);
        var response = await DeleteAsync($"api/producttypes/{id}");

        if (response.IsSuccessStatusCode)
            return (true, null);

        var content = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            _logger.LogWarning("DeleteProductTypeAsync conflict for {Id}: {Content}", id, content);
            return (false, string.IsNullOrWhiteSpace(content) ? "Cannot delete a product type that still has products." : content);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return (false, null);

        _logger.LogWarning("Failed to delete product type {ProductTypeId}. Status: {StatusCode}, Content: {Content}", id, response.StatusCode, content);
        return (false, "Unable to delete the product type.");
    }
}
