using System.ComponentModel.DataAnnotations;

namespace VariBillWebApp.Mvc.Models.ViewModels;

/// <summary>
/// View model for editing a product.
/// </summary>
public class ProductEditViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters.")]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "SKU must be between 3 and 50 characters.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "SKU must be uppercase alphanumeric with hyphens.")]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 9999999.99, ErrorMessage = "Price must be between 0.01 and 9,999,999.99.")]
    [Display(Name = "Price")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    [Display(Name = "Quantity")]
    public int Quantity { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    [Display(Name = "Description")]
    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Product type is required.")]
    [Display(Name = "Product Type")]
    public Guid ProductTypeId { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }
}
