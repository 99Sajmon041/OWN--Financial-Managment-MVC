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
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<FinancialManagementDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "FinancialManagement.Dev.Auth";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";

    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\simon\Desktop\FinancialManagment\AppKeys\FinancialManagement_Dev"))
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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestMonitoringMiddleware>();

app.MapDefaultControllerRoute();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
