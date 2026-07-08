//using VariBillWebAPI.Data.Entities;
//using VariBillWebAPI.Data.UnitOfWork.Interfaces;
//using VariBillWebAPI.Models.DTO;
//using VariBillWebAPI.Services.Interfaces;

//namespace VariBillWebAPI.Services;

//public class ProductService : IProductService
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly ILogger<ProductService> _logger;

//    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
//    {
//        _unitOfWork = unitOfWork;
//        _logger = logger;
//    }

//    public async Task<IReadOnlyList<ProductResponseDto>> GetAllAsync()
//    {
//        var products = await _unitOfWork.Products.GetAllWithProductTypeAsync();
//        return products.Select(MapToDto).ToList();
//    }

//    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
//    {
//        var product = await _unitOfWork.Products.GetByIdWithProductTypeAsync(id);
//        return product is null ? null : MapToDto(product);
//    }

//    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
//    {
//        if (!await ProductTypeExistsAsync(dto.ProductTypeId))
//        {
//            throw new InvalidOperationException($"Product type '{dto.ProductTypeId}' was not found.");
//        }

//        var product = new Product
//        {
//            Id = Guid.NewGuid(),
//            Name = dto.Name.Trim(),
//            Price = dto.Price,
//            Description = dto.Description?.Trim(),
//            ProductTypeId = dto.ProductTypeId,
//            DateCreated = DateTime.UtcNow
//        };

//        await _unitOfWork.Products.AddAsync(product);
//        await _unitOfWork.SaveChangesAsync();

//        var created = await _unitOfWork.Products.GetByIdWithProductTypeAsync(product.Id)
//            ?? product;

//        return MapToDto(created);
//    }

//    public async Task<bool> UpdateAsync(Guid id, UpdateProductDto dto)
//    {
//        var product = await _unitOfWork.Products.FindAsync(id);
//        if (product is null)
//        {
//            return false;
//        }

//        if (!await ProductTypeExistsAsync(dto.ProductTypeId))
//        {
//            throw new InvalidOperationException($"Product type '{dto.ProductTypeId}' was not found.");
//        }

//        product.Name = dto.Name.Trim();
//        product.Price = dto.Price;
//        product.Description = dto.Description?.Trim();
//        product.ProductTypeId = dto.ProductTypeId;
//        product.DateModified = DateTime.UtcNow;

//        await _unitOfWork.Products.UpdateAsync(product);
//        await _unitOfWork.SaveChangesAsync();
//        return true;
//    }

//    public async Task<bool> DeleteAsync(Guid id)
//    {
//        var product = await _unitOfWork.Products.FindAsync(id);
//        if (product is null)
//        {
//            return false;
//        }

//        await _unitOfWork.Products.DeleteAsync(product);
//        await _unitOfWork.SaveChangesAsync();
//        return true;
//    }

//    private async Task<bool> ProductTypeExistsAsync(Guid productTypeId)
//    {
//        return await _unitOfWork.ProductTypes.FindAsync(productTypeId) is not null;
//    }

//    private static ProductResponseDto MapToDto(Product product) =>
//        new(
//            product.Id,
//            product.Name,
//            product.Price,
//            product.Description,
//            product.ProductTypeId,
//            product.ProductType?.Name);
//}
