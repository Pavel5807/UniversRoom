using System.Security.Claims;
using Duende.IdentityServer;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversRoom.Services.Identity.API.Models;

namespace UniversRoom.Services.Identity.API.Controllers;

public class ExternalController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExternalController(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Challenge(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "~/";
        }

        if (Url.IsLocalUrl(returnUrl) == false)
        {
            throw new Exception("invalid return URL");
        }

        var props = new AuthenticationProperties()
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", scheme }
            }
        };

        return Challenge(props, scheme);
    }

    [HttpGet]
    public async Task<IActionResult> Callback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(
            IdentityServerConstants.ExternalCookieAuthenticationScheme);

        if (authenticateResult.Succeeded is false)
        {
            throw new Exception();
        }

        var user = await GetUserAsync(authenticateResult) ??
            await CreateUserAsync(authenticateResult);
        var authenticationProperties = GetLocalSigninProps(authenticateResult);
        var additionalClaims = GetAdditionalLocalClaims(authenticateResult);

        await _signInManager.SignInWithClaimsAsync(user, authenticationProperties, additionalClaims);

        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        var returnUrl = authenticateResult.Properties.Items["returnUrl"] ?? "~/";

        return Redirect(returnUrl);
    }

    private async Task<ApplicationUser?> GetUserAsync(AuthenticateResult authenticate)
    {
        if (authenticate.Principal is null
            || authenticate.Properties is null)
        {
            throw new Exception();
        }

        var userIdClaim = authenticate.Principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new Exception();

        var provider = authenticate.Properties.Items["scheme"]!;
        var providerKey = userIdClaim.Value;

        return await _userManager.FindByLoginAsync(provider, providerKey);
    }

    private async Task<ApplicationUser> CreateUserAsync(AuthenticateResult authenticate)
    {
        if (authenticate.Principal is null
            || authenticate.Properties is null)
        {
            throw new Exception();
        }

        var userIdClaim = authenticate.Principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new Exception();

        var provider = authenticate.Properties.Items["scheme"]!;
        var providerKey = userIdClaim.Value;

        var sub = Guid.NewGuid().ToString();
        var email = authenticate.Principal.Claims.FirstOrDefault(x => x.Type is ClaimTypes.Email)?.Value;

        var user = new ApplicationUser()
        {
            Id = sub,
            Email = email,
            UserName = sub
        };

        var claims = new List<Claim>();
        if (authenticate.Principal.Claims.FirstOrDefault(x => x.Type is ClaimTypes.Name) is Claim claim)
        {
            claims.Add(new Claim(JwtClaimTypes.Name, claim.Value));
        }

        var identityResult = await _userManager.CreateAsync(user);
        if (identityResult.Succeeded is false)
        {
            throw new Exception();
        }

        identityResult = await _userManager.AddClaimsAsync(user, claims);
        if (identityResult.Succeeded is false)
        {
            throw new Exception();
        }

        identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
        if (identityResult.Succeeded is false)
        {
            throw new Exception();
        }

        return user;
    }

    private AuthenticationProperties GetLocalSigninProps(AuthenticateResult authenticate)
    {
        if (authenticate.Properties is null)
        {
            throw new Exception();
        }

        var idToken = authenticate.Properties.GetTokenValue("id_token");
        if (idToken is null)
        {
            return new AuthenticationProperties();
        }

        var localSigninProps = new AuthenticationProperties();
        localSigninProps.StoreTokens(new[]
        {
            new AuthenticationToken()
            {
                Name = "id_token",
                Value = idToken
            }
        });

        return localSigninProps;
    }

    private List<Claim> GetAdditionalLocalClaims(AuthenticateResult authenticate)
    {
        if (authenticate.Principal is null
            || authenticate.Properties is null)
        {
            throw new Exception();
        }

        var provider = authenticate.Properties.Items["scheme"]!;

        var additionalLocalClaims = new List<Claim>
        {
            new(JwtClaimTypes.IdentityProvider, provider)
        };

        if (authenticate.Principal.Claims.FirstOrDefault(x => x.Type is JwtClaimTypes.SessionId) is Claim claim)
        {
            additionalLocalClaims.Add(new Claim(JwtClaimTypes.SessionId, claim.Value));
        }

        return additionalLocalClaims;
    }
}