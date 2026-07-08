using System.ComponentModel.DataAnnotations;

namespace VariBillWebAPI.Models.DTO;

public record ProductResponseDto(
    Guid Id,
    string Name,
    string SKU,
    decimal Price,
    int Quantity,
    string? Description,
    Guid ProductTypeId,
    string? ProductTypeName,
    bool IsActive,
    DateTime DateCreated,
    DateTime? DateModified);

public record CreateProductDto(
    [property: Required, MaxLength(200)] string Name,
    [property: Required, MaxLength(50)] string SKU,
    [property: Range(0.01, double.MaxValue)] decimal Price,
    [property: Range(0, int.MaxValue)] int Quantity,
    string? Description,
    [property: Required] Guid ProductTypeId);

public record UpdateProductDto(
    Guid Id,
    [property: Required, MaxLength(200)] string Name,
    [property: Required, MaxLength(50)] string SKU,
    [property: Range(0.01, double.MaxValue)] decimal Price,
    [property: Range(0, int.MaxValue)] int Quantity,
    string? Description,
    [property: Required] Guid ProductTypeId,
    bool IsActive);




