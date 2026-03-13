namespace FinancialManagment.Application.UserIdentity;

public interface ICurrentUser
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
    string ValidatedUserId { get; }
}
