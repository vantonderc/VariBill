namespace VariBillWebApp.Mvc.Models.ViewModels;

/// <summary>
/// View model for displaying a product.
/// </summary>
public class ProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public Guid ProductTypeId { get; set; }
    public string? ProductTypeName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
