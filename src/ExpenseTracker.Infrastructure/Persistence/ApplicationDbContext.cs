using ExpenseTracker.Core.Common;
using ExpenseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExpenseTracker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Receipt> Receipts => Set<Receipt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            string connString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connString); //Or whatever DB you are using
        }
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "system"; // TODO: Get current user
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = "system"; // TODO: Get current user
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateDefaultCategoriesForUserAsync(Guid userId)
    {
        // Execute the stored procedure to create default categories for the user
        await Database.ExecuteSqlRawAsync(
            "EXEC [dbo].[CreateDefaultCategoriesForUser] @UserId",
            new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId));
    }
}
