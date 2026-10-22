using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Application.Authentication;

public interface IUserRepository
{
    User? FindByUsername(string username);
    bool TryAdd(User user);
}
