namespace VariBillWebAPI.Services.Interfaces;

/// <summary>
/// Provides the current user's identity information.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID (or "System" if not authenticated).
    /// </summary>
    string UserId { get; }
}