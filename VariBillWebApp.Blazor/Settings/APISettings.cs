namespace VariBillWebApp.Blazor.Settings;

/// <summary>
/// Configuration settings for API connections and authentication.
/// </summary>
public class APISettings
{
    /// <summary>
    /// Gets or sets the VariBill API base URL.
    /// </summary>
    public string VariBillAPIAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Identity Server URL.
    /// </summary>
    public string IdentityServer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client ID for client‑credentials (machine‑to‑machine) flow.
    /// </summary>
    public string APIClient { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the interactive client ID for Authorization Code + PKCE flow.
    /// </summary>
    public string InteractiveClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API client secret.
    /// </summary>
    public string APISecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API scope requested.
    /// </summary>
    public string APIScope { get; set; } = string.Empty;
}