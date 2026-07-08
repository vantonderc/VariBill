//using VariBillWebAPI.Data.Entities;
//using VariBillWebAPI.Data.UnitOfWork.Interfaces;
//using VariBillWebAPI.Models.DTO;
//using VariBillWebAPI.Services.Interfaces;

//namespace VariBillWebAPI.Services;

//public class ProductTypeService : IProductTypeService
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly ILogger<ProductTypeService> _logger;

//    public ProductTypeService(IUnitOfWork unitOfWork, ILogger<ProductTypeService> logger)
//    {
//        _unitOfWork = unitOfWork;
//        _logger = logger;
//    }

//    public async Task<IReadOnlyList<ProductTypeResponseDto>> GetAllAsync()
//    {
//        var productTypes = await _unitOfWork.ProductTypes.GetAllWithProductCountsAsync();
//        return productTypes
//            .Select(pt => MapToDto(pt, pt.Products?.Count ?? 0))
//            .ToList();
//    }

//    public async Task<ProductTypeResponseDto?> GetByIdAsync(Guid id)
//    {
//        var productType = await _unitOfWork.ProductTypes.GetByIdWithProductsAsync(id);
//        return productType is null
//            ? null
//            : MapToDto(productType, productType.Products.Count);
//    }

//    public async Task<ProductTypeResponseDto> CreateAsync(CreateProductTypeDto dto)
//    {
//        var productType = new ProductType
//        {
//            Id = Guid.NewGuid(),
//            Name = dto.Name.Trim(),
//            Description = dto.Description?.Trim()
//        };

//        await _unitOfWork.ProductTypes.AddAsync(productType);
//        await _unitOfWork.SaveChangesAsync();

//        return MapToDto(productType, 0);
//    }

//    public async Task<bool> UpdateAsync(Guid id, UpdateProductTypeDto dto)
//    {
//        var productType = await _unitOfWork.ProductTypes.FindAsync(id);
//        if (productType is null)
//        {
//            return false;
//        }

//        productType.Name = dto.Name.Trim();
//        productType.Description = dto.Description?.Trim();

//        await _unitOfWork.ProductTypes.UpdateAsync(productType);
//        await _unitOfWork.SaveChangesAsync();
//        return true;
//    }

//    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
//    {
//        var productType = await _unitOfWork.ProductTypes.FindAsync(id);
//        if (productType is null)
//        {
//            return (false, null);
//        }

//        if (await _unitOfWork.ProductTypes.HasProductsAsync(id))
//        {
//            return (false, "Cannot delete a product type that still has products.");
//        }

//        await _unitOfWork.ProductTypes.DeleteAsync(productType);
//        await _unitOfWork.SaveChangesAsync();
//        return (true, null);
//    }

//    private static ProductTypeResponseDto MapToDto(ProductType productType, int productCount) =>
//        new(productType.Id, productType.Name, productType.Description, productCount);
//}
