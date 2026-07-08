namespace VariBillWebApp.Blazor.Models.DTOs;

/// <summary>
/// Product DTO received from the API.
/// </summary>
public record ProductApiResponseDto(
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

/// <summary>
/// Product DTO sent to the API for create/update.
/// </summary>
public record ProductApiWriteDto(
    string Name,
    decimal Price,
    string? Description,
    Guid ProductTypeId);

/// <summary>
/// Product type DTO received from the API.
/// </summary>
public record ProductTypeApiResponseDto(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount);

/// <summary>
/// DTO for creating a product type.
/// </summary>
public record CreateProductTypeDto(
    string Name,
    string? Description);

/// <summary>
/// DTO for updating a product type.
/// </summary>
public record UpdateProductTypeDto(
    string Name,
    string? Description);