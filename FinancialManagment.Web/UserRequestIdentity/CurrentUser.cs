using FinancialManagment.Application.UserIdentity;
using System.Security.Claims;

namespace FinancialManagment.Infrastructure.Identity;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string ValidatedUserId
    {
        get
        {
            string? userId = UserId;

            if (IsAuthenticated && !string.IsNullOrWhiteSpace(userId))
                return userId;
            else
                throw new UnauthorizedAccessException("Operace nelze provést - uživatelské ID nebylo rozpoznáno.");
        }
    }
}
