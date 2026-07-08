using System.ComponentModel.DataAnnotations;

namespace VariBillWebApp.Blazor.Models.ViewModels;

public class ProductTypeFormViewModel
{
    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }
}
