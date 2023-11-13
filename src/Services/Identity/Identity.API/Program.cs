using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using UniversRoom.Services.Identity.API.Configuration;
using UniversRoom.Services.Identity.API.Data;
using UniversRoom.Services.Identity.API.Models;
using Duende.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = "null";
    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
})
.AddAspNetIdentity<ApplicationUser>()
.AddInMemoryApiResources(Configuration.GetApiResources())
.AddInMemoryApiScopes(Configuration.GetApiScopes())
.AddInMemoryClients(Configuration.GetClients())
.AddInMemoryIdentityResources(Configuration.GetIdentityResources())
.AddDeveloperSigningCredential();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        
        options.ClientId = "711557403775-9v3jllb5dlvdqtusn33kogh850ohgkqf.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });

builder.Services.ConfigureApplicationCookie(configure =>
{
    configure.Cookie.Name = "UniversRoom.Identity.Cookie";
    configure.LoginPath = "/Account/Login";
    configure.LogoutPath = "/Account/Logout";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});
app.UseIdentityServer();
app.UseAuthorization();
app.MapDefaultControllerRoute();

app.Run();
