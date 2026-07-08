using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Bongoe.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("BongoeWebServiceDemo", "Bongoe Web Service Access"),
                new ApiScope("FileUploader", "File Uploader Access")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                // This links the "Audience" name to the "Scopes"
                new ApiResource("BongoeWebServiceDemo", "Bongoe Web Service")
                {
                    Scopes = { "BongoeWebServiceDemo" }
                },
                new ApiResource("FileUploader", "File Uploader Resource")
                {
                    Scopes = { "FileUploader" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "BongoeWebAppDemo",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("G3cX6Dt9JhUmaZ8F".Sha256()) },
                    AllowedScopes = { "BongoeWebServiceDemo" }
                },
                new Client
                {
                    ClientId = "BongoeWebServiceDemo",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("G3cX6Dt9JhUmaZ8F".Sha256()) },
                    AllowedScopes = { "FileUploader" }
                },
                new Client
                {
                    ClientId = "GradingScheduler",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("G3cX6Dt9JhUmaZ8F".Sha256()) },
                    AllowedScopes = { "BongoeWebServiceDemo" }
                }
            };
    }
}