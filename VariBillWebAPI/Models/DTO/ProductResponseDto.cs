//using System.ComponentModel.DataAnnotations;

//namespace VariBillWebAPI.Models.DTO;

//public record ProductResponseDto(
//    Guid Id,
//    string Name,
//    decimal Price,
//    string? Description,
//    Guid ProductTypeId,
//    string? ProductTypeName);

//public record CreateProductDto(
//    [property: Required, MaxLength(200)] string Name,
//    [property: Range(0.01, double.MaxValue)] decimal Price,
//    string? Description,
//    Guid ProductTypeId);

//public record UpdateProductDto(
//    [property: Required, MaxLength(200)] string Name,
//    [property: Range(0.01, double.MaxValue)] decimal Price,
//    string? Description,
//    Guid ProductTypeId);

//public record ProductTypeResponseDto(
//    Guid Id,
//    string Name,
//    string? Description,
//    int ProductCount);

//public record CreateProductTypeDto(
//    [property: Required, MaxLength(100)] string Name,
//    string? Description);

//public record UpdateProductTypeDto(
//    [property: Required, MaxLength(100)] string Name,
//    string? Description);
