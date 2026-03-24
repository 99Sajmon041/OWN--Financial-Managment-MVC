using FinancialManagment.Application.Services.Interfaces;

namespace FinancialManagment.Web.Image;

public sealed class ImageService(IWebHostEnvironment environment) : IImageService
{
    private const string Imagesfolder = "Images/ExpenseReceipts";
    public async Task<string?> SaveAsync(IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var uploadsPath = Path.Combine(environment.WebRootPath, Imagesfolder);
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

        var filePath = Path.Combine(environment.WebRootPath, Imagesfolder, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
