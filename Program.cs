using BlazorADAuth.Components;
using BlazorADAuth.Components.Account;
using BlazorADAuth.Contracts;
using BlazorADAuth.Data;
using BlazorADAuth.Entities.Auth;
using BlazorADAuth.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Sqids;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IServiceCollection? services = builder.Services;

    // Add services to the container.
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration));

    services.AddRazorComponents()
        .AddInteractiveServerComponents();

    services.AddCascadingAuthenticationState();
    services.AddScoped<IAdUserService, AdUserService>();
    services.AddScoped<IdentityUserAccessor>();
    services.AddScoped<IdentityRedirectManager>();
    services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

    services.AddSingleton(new SqidsEncoder<int>(new()
    {
        Alphabet = "scXxRo4blWiUfaNOuGnzDE5j7wBA6vkm309JV18rThMqPdLHgSyFZQIpYteKC2",
        MinLength = 8,
    }));

    services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    }).AddIdentityCookies();
    
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.User.RequireUniqueEmail = true;
    })
        .AddUserManager<AppUserManager<ApplicationUser>>()
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager<AppSignInManager<ApplicationUser>>()
        .AddDefaultTokenProviders();

    services.ConfigureApplicationCookie(options =>
    {
        options.AccessDeniedPath = "/account/access-denied";
        options.Cookie.Name = "BlazorADAuth.Cookie";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = new PathString("/account/login");
        options.LogoutPath = new PathString("/account/logout");
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
        options.SlidingExpiration = true;
    });

    if (builder.Environment.IsDevelopment())
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

#if DEBUG
    if (app.Environment.IsDevelopment())
    {
        bool seed = app.Configuration.GetValue<bool>("seed");
        if (seed)
        {
            using IServiceScope? scope = app.Services.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using AppUserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<AppUserManager<ApplicationUser>>();
            using RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            DbSeed.SeedDb(context, userManager, roleManager);
        }
    }
#endif

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseSerilogRequestLogging();
    app.UseAntiforgery();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .RequireAuthorization();

    // Add additional endpoints required by the Identity /Account Razor components.
    app.MapAdditionalIdentityEndpoints();

    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
        throw;
    Log.Fatal(ex, "A fatal error occurred: {exMessage}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}
