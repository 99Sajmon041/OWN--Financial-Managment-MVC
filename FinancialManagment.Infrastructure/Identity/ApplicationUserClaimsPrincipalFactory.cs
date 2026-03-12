using System.Security.Claims;
using FinancialManagment.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace FinancialManagment.Infrastructure.Identity;

public sealed class ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim("first_name", user.FirstName ?? string.Empty));
        identity.AddClaim(new Claim("last_name", user.LastName ?? string.Empty));
        identity.AddClaim(new Claim("full_name", $"{user.FirstName} {user.LastName}".Trim()));

        return identity;
    }
}
