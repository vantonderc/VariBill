using System.ComponentModel.DataAnnotations;

namespace VariBillWebAPI.Data.Entities;

/// <summary>
/// Represents a product.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    public Guid ProductTypeId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int Quantity { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime? DateModified { get; set; }

    // Navigation properties (virtual for lazy loading)
    /// <summary>
    /// Navigation property to the product type.
    /// </summary>
    public virtual ProductType? ProductType { get; set; }
}



