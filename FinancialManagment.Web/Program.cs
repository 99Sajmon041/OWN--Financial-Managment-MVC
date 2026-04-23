using FinancialManagment.Application.Extensions;
using FinancialManagment.Application.Services.Interfaces;
using FinancialManagment.Application.UserIdentity;
using FinancialManagment.Domain.Entities;
using FinancialManagment.Infrastructure.Database;
using FinancialManagment.Infrastructure.Extensions;
using FinancialManagment.Infrastructure.Identity;
using FinancialManagment.Web.Image;
using FinancialManagment.Web.LoggingService;
using FinancialManagment.Web.MiddleWare;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IRequestMonitoringLogService, RequestMonitoringLogService>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<FinancialManagementDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("AuthPolicy", HttpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "FinancialManagement.Dev.Auth";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";

    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\AppKeys\FinancialManagement_Dev"))
    .SetApplicationName("FinancialManagement.Dev");

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "FinancialManagement.Dev.Antiforgery";
});

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestMonitoringMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapDefaultControllerRoute();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
