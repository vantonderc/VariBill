using System.ComponentModel.DataAnnotations;

namespace VariBillWebApp.Mvc.Models.ViewModels;

/// <summary>
/// View model for displaying a product type.
/// </summary>
public class ProductTypeViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductCount { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// View model for creating a product type.
/// </summary>
public class CreateProductTypeModel
{
    [Required(ErrorMessage = "Product type name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    [Display(Name = "Type Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }
}

/// <summary>
/// View model for updating a product type.
/// </summary>
public class UpdateProductTypeModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product type name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    [Display(Name = "Type Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}





