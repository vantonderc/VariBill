//using System.ComponentModel.DataAnnotations;

//namespace VariBillWebAPI.Data.Entities;

//public class Product
//{
//    [Key]
//    public Guid Id { get; set; }

//    public Guid ProductTypeId { get; set; }

//    [Required]
//    [MaxLength(200)]
//    public string Name { get; set; } = string.Empty;

//    public decimal Price { get; set; }

//    [MaxLength(1000)]
//    public string? Description { get; set; }

//    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

//    public DateTime? DateModified { get; set; }

//    public virtual ProductType? ProductType { get; set; }
//}
