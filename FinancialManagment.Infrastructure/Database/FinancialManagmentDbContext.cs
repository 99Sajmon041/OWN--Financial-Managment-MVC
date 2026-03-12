using FinancialManagment.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagment.Infrastructure.Database;

public sealed class FinancialManagmentDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Expense> Expenses { get; set; } = default!;
    public DbSet<Income> Incomes { get; set; } = default!;
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; } = default!;
    public DbSet<IncomeCategory> IncomeCategories { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Expense>(entity =>
        {
            entity.Property(x => x.Description)
                .HasMaxLength(200);

            entity.Property(x => x.ReceiptFileName)
                .HasMaxLength(200);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.HasIndex(x => new { x.ApplicationUserId, x.Date, x.ExpenseCategoryId });
        });

        builder.Entity<Income>(entity =>
        {
            entity.Property(x => x.Description)
                .HasMaxLength(200);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.HasIndex(x => new { x.ApplicationUserId, x.Date, x.IncomeCategoryId });

        });

        builder.Entity<ExpenseCategory>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(200);

            entity.HasMany(x => x.Expenses)
                .WithOne(x => x.ExpenseCategory)
                .HasForeignKey(x => x.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ApplicationUserId, x.Name }).IsUnique();
        });

        builder.Entity<IncomeCategory>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(200);

            entity.HasMany(x => x.Incomes)
                .WithOne(x => x.IncomeCategory)
                .HasForeignKey(x => x.IncomeCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.ApplicationUserId, x.Name }).IsUnique();
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.FirstName)
                .HasMaxLength(200);

            entity.Property(x => x.LastName)
                .HasMaxLength(200);

            entity.HasMany(x => x.IncomeCategories)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.ExpenseCategories)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Incomes)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict); 

            entity.HasMany(x => x.Expenses)
                .WithOne(x => x.ApplicationUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict); 
        });
    }
}
