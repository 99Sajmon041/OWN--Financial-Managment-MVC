using Microsoft.AspNetCore.Http;

namespace FinancialManagment.Application.Services.Interfaces;

public interface IImageService
{
    Task<string?> SaveAsync(IFormFile? file, CancellationToken ct);
    Task DeleteAsync(string? fileName, CancellationToken ct);
}
