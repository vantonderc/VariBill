namespace VariBillWebApp.Mvc.Models.ViewModels;

/// <summary>
/// View model for a product.
/// </summary>
public class ProductModel
{
    /// <summary>Gets or sets the product ID.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the product name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the product price.</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the product description.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the product type ID.</summary>
    public Guid ProductTypeId { get; set; }

    /// <summary>Gets or sets the product type name.</summary>
    public string? ProductTypeName { get; set; }
}

/// <summary>DTO for creating a product.</summary>
public class CreateProductModel
{
    /// <summary>Product name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Product price.</summary>
    public decimal Price { get; set; }

    /// <summary>Product description.</summary>
    public string? Description { get; set; }

    /// <summary>Product type ID.</summary>
    public Guid ProductTypeId { get; set; }
}

/// <summary>DTO for updating a product.</summary>
public class UpdateProductModel
{
    /// <summary>Product name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Product price.</summary>
    public decimal Price { get; set; }

    /// <summary>Product description.</summary>
    public string? Description { get; set; }

    /// <summary>Product type ID.</summary>
    public Guid ProductTypeId { get; set; }
}

//namespace VariBillWebApp.Mvc.Models.ViewModels;

//public class ProductModel
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; } = string.Empty;
//    public decimal Price { get; set; }
//    public string? Description { get; set; }
//    public Guid ProductTypeId { get; set; }
//    public string? ProductTypeName { get; set; }
//}

//public class CreateProductModel
//{
//    public string Name { get; set; } = string.Empty;
//    public decimal Price { get; set; }
//    public string? Description { get; set; }
//    public Guid ProductTypeId { get; set; }
//}

//public class UpdateProductModel
//{
//    public string Name { get; set; } = string.Empty;
//    public decimal Price { get; set; }
//    public string? Description { get; set; }
//    public Guid ProductTypeId { get; set; }
//}
