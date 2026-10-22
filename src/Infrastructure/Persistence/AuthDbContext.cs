using Microsoft.EntityFrameworkCore;
using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Infrastructure.Persistence;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Usa o nome normalizado como chave: user e USER devem representar a mesma conta.
        modelBuilder.Entity<User>().HasKey(user => user.NormalizedUsername);
        modelBuilder.Entity<User>().Property(user => user.Username).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<User>().Property(user => user.PasswordHash).IsRequired();
    }
}
