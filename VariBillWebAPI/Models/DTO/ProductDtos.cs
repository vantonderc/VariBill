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

//public record CreateProductDto(
//    [property: Required, MaxLength(200)] string Name,
//    [property: Required, MaxLength(50)] string SKU,
//    [property: Range(0.01, double.MaxValue)] decimal Price,
//    [property: Range(0, int.MaxValue)] int Quantity,
//    string? Description,
//    [property: Required] Guid ProductTypeId);

public class CreateProductDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; }

    [Required, MaxLength(50)]
    public string SKU { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    public string? Description { get; set; }

    [Required]
    public Guid ProductTypeId { get; set; }
}
//public record UpdateProductDto(
//    Guid Id,
//    [property: Required, MaxLength(200)] string Name,
//    [property: Required, MaxLength(50)] string SKU,
//    [property: Range(0.01, double.MaxValue)] decimal Price,
//    [property: Range(0, int.MaxValue)] int Quantity,
//    string? Description,
//    [property: Required] Guid ProductTypeId,
//    bool IsActive);

public record UpdateProductDto(
    Guid Id,
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(50)] string SKU,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int Quantity,
    string? Description,
    [Required] Guid ProductTypeId,
    bool IsActive);



