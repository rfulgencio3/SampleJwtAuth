using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Application.Authentication;

public interface IPasswordService
{
    string Hash(User user, string password);
    bool Verify(User user, string password);
}
