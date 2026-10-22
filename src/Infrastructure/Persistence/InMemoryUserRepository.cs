using Microsoft.EntityFrameworkCore;
using SampleJwtAuth.Application.Authentication;
using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Infrastructure.Persistence;

public sealed class InMemoryUserRepository(AuthDbContext database) : IUserRepository
{
    private static readonly object RegistrationLock = new();

    public User? FindByUsername(string username)
    {
        // Aplica a mesma normalização do cadastro e consulta sem rastrear alterações na entidade.
        var normalized = username.Trim().ToUpperInvariant();
        return database.Users.AsNoTracking().SingleOrDefault(user => user.NormalizedUsername == normalized);
    }

    public bool TryAdd(User user)
    {
        // Como o InMemory não oferece restrições de unicidade como um banco relacional,
        // o lock impede dois cadastros simultâneos do mesmo nome neste processo.
        lock (RegistrationLock)
        {
            if (database.Users.Any(existing => existing.NormalizedUsername == user.NormalizedUsername))
                return false;

            database.Users.Add(user);
            // Add prepara a inclusão; SaveChanges efetiva a gravação no banco em memória.
            database.SaveChanges();
            return true;
        }
    }
}
