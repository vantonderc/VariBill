namespace VariBillWebApp.Mvc.Models.ViewModels;

/// <summary>
/// View model for the error page.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}



