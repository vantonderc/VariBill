using System.ComponentModel.DataAnnotations;

namespace VariBillWebAPI.Models.DTO;

public record ProductTypeResponseDto(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount);

public record CreateProductTypeDto(
    [property: Required, MaxLength(100)] string Name,
    string? Description);

public record UpdateProductTypeDto(
    Guid Id,
    [property: Required, MaxLength(100)] string Name,
    string? Description,
    bool IsActive);

