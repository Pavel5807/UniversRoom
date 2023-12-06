using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversRoom.Services.Identity.API.Models;
using UniversRoom.Services.Identity.API.Models.AccountViewModels;

namespace UniversRoom.Services.Identity.API.Controllers;

public class AccountController : Controller
{
    private readonly IIdentityServerInteractionService _interactionService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public AccountController(
        IIdentityServerInteractionService interactionService,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IAuthenticationSchemeProvider schemeProvider)
    {
        _interactionService = interactionService;
        _signInManager = signInManager;
        _userManager = userManager;
        _schemeProvider = schemeProvider;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string? returnUrl)
    {
        var model = await BuildLoginViewModel(returnUrl);

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid is false)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "user not found");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
        if (result.Succeeded is false)
        {
            ModelState.AddModelError(string.Empty, "login error");
            return View(model);
        }

        return Redirect(model.ReturnUrl ?? "~/");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl)
    {
        var model = new RegisterViewModel()
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid is false)
        {
            return View(model);
        }

        var user = new ApplicationUser()
        {
            Email = model.Email,
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded is false)
        {
            ModelState.AddModelError(string.Empty, "Error occurred");
            return View(model);
        }

        await _signInManager.SignInAsync(user, true);

        return Redirect(model.ReturnUrl ?? "~/");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LogoutAsync(string logoutId)
    {
        await _signInManager.SignOutAsync();
        var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);
        return Redirect(logoutRequest.PostLogoutRedirectUri ?? "~/");
    }

    private async Task<LoginViewModel> BuildLoginViewModel(string? returnUrl)
    {
        var context = _interactionService.GetAuthorizationContextAsync(returnUrl);

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        return new LoginViewModel()
        {
            ReturnUrl = returnUrl,
        };
    }
}