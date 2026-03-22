using FinancialManagment.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace FinancialManagment.Application.Services.Implementations;

public sealed class ImageService(IHostEnvironment environment) : IImageService
{
    private const string Imagesfolder = "Images/ExpenseReceipts";
    public async Task<string?> SaveAsync(IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var uploadsPath = Path.Combine(environment.ContentRootPath, Imagesfolder);
        Directory.CreateDirectory(uploadsPath);

        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return uniqueFileName;
    }
    public Task DeleteAsync(string? fileName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Task.CompletedTask;
        }

        var filePath = Path.Combine(environment.ContentRootPath, Imagesfolder, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
