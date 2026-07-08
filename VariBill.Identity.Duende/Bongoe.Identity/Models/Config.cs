using Duende.IdentityServer.Models;
using static System.Net.WebRequestMethods;

namespace Bongoe.Identity;

/// <summary>
/// IdentityServer configuration: resources, scopes, and clients.
/// </summary>
public static class Config
{
    /// <summary>
    /// Identity resources (OpenID Connect standard + custom roles).
    /// </summary>
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "roles",
                UserClaims = new[] { "role" }
            }
        };

    /// <summary>
    /// API scopes.
    /// </summary>
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("VariBillWebAPI", "VariBill Web API")
        };

    /// <summary>
    /// API resources (include role claim in access token).
    /// </summary>
    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("VariBillWebAPI", "VariBill Web API")
            {
                Scopes = { "VariBillWebAPI" },
                UserClaims = { "role" }
            }
        };

    /// <summary>
    /// Clients (machine‑to‑machine and interactive).
    /// </summary>
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // ----- Machine‑to‑machine (client credentials) -----
            new Client
            {
                ClientId = "VariBillWebAPI.ClientCredentials",
                ClientName = "VariBill Web API (Machine-to-Machine)",
                ClientSecrets = { new Secret("G3cX6Dt9JhUmaZ8F".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "VariBillWebAPI" }
            },

            // ----- Interactive client for MVC / Blazor (Authorization Code + PKCE) -----
            new Client
            {
                ClientId = "VariBillWebApp.Interactive",
                ClientName = "VariBill Web App (Interactive)",
                ClientSecrets = { new Secret("G3cX6Dt9JhUmaZ8F".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = true,
                RedirectUris = new[]
                {
                    "https://localhost:5005/signin-oidc",  
                    "https://localhost:5006/signin-oidc",
                    "https://localhost:7253/signin-oidc",   // MVC
                    "https://localhost:7055/signin-oidc"    // Blazor

                },
                PostLogoutRedirectUris = new[]
                {
                    "https://localhost:5005/signout-callback-oidc",
                    "https://localhost:5006/signout-callback-oidc",
                    "https://localhost:7253/signout-callback-oidc", 
                    "https://localhost:7055/signout-callback-oidc"   
                },
                AllowedScopes = new[]
                {
                    "openid",
                    "profile",
                    "email",
                    "roles",
                    "VariBillWebAPI"
                },
                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                AccessTokenLifetime = 3600, // 1 hour
                IdentityTokenLifetime = 300
            }
        };
}
