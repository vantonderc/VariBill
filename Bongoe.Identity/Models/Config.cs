using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Bongoe.Identity;

/// <summary>
/// IdentityServer configuration: resources, scopes, and clients.
/// </summary>
public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles", new[] { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("VariBillWebAPI", "VariBill Web API")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("VariBillWebAPI", "VariBill Web API")
            {
                Scopes = { "VariBillWebAPI" },
                UserClaims = { "role" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "VariBillWebAPI.ClientCredentials",
                ClientName = "VariBill Web API (Machine-to-Machine)",
                ClientSecrets = { new Secret("G3cX6Dt9JhUmaZ8F".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "VariBillWebAPI" }
            },

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
                    "https://localhost:7253/signin-oidc", // MVC
                    "https://localhost:7055/signin-oidc"  // Blazor
                },
                PostLogoutRedirectUris = new[]
                {
                    "https://localhost:7253/signout-callback-oidc",
                    "https://localhost:7055/signout-callback-oidc"
                },
                AllowedScopes = new[] { "openid", "profile", "email", "roles", "VariBillWebAPI" },
                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                AccessTokenLifetime = 3600,
                IdentityTokenLifetime = 300
            }
        };
}
