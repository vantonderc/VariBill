namespace VariBillWebApp.Mvc.Settings;

/// <summary>
/// Represents settings for API connections and authentication.
/// </summary>
public class APISettings
{
    /// <summary>
    /// Gets or sets the VariBill API address.
    /// </summary>
    public string VariBillAPIAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Identity Server address.
    /// </summary>
    public string IdentityServer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API client ID for machine‑to‑machine (client credentials) flow.
    /// </summary>
    public string APIClient { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the interactive client ID used for Authorization Code + PKCE flow.
    /// </summary>
    public string InteractiveClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API client secret.
    /// </summary>
    public string APISecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API scope.
    /// </summary>
    public string APIScope { get; set; } = string.Empty;
}
