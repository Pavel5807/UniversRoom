using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace UniversRoom.Services.Identity.API.Configuration;

public static class Configuration
{
    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>()
        {
            new ApiResource("WebMVC", "Web MVC", new [] { JwtClaimTypes.Name })
            {
                Scopes = {
                    "WebMVC"
                }
            }
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>()
        {
            new ApiScope("WebMVC", "Web MVC")
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>()
        {
            new Client()
            {
                ClientId = "web-mvc",
                ClientName = "Web",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RedirectUris = 
                {
                    "http://localhost:5271/signin-oidc"
                },
                AllowedCorsOrigins =
                {
                    "http://localhost:5271"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:5271/signout-oidc"
                },
                AllowedScopes = 
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "WebMVC"
                },
                AllowAccessTokensViaBrowser = true
            }
        };
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

}