
using System.ComponentModel.DataAnnotations;

namespace VariBillWebAPI.Data.Entities;

/// <summary>
/// Represents a product type/category.
/// </summary>
public class ProductType
{
    /// <summary>
    /// Gets or sets the unique identifier of the product type.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    /// <summary>
    /// Gets or sets the products associated with this product type.
    /// </summary>
}



