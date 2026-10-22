using Microsoft.AspNetCore.Identity;
using SampleJwtAuth.Application.Authentication;
using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Infrastructure.Authentication;

public sealed class PasswordService(IPasswordHasher<User> hasher) : IPasswordService
{
    // O PasswordHasher inclui salt e parâmetros de derivação no resultado armazenado.
    public string Hash(User user, string password) => hasher.HashPassword(user, password);

    // A verificação não recupera a senha original. SuccessRehashNeeded também indica senha correta;
    // neste exemplo, o hash não é atualizado automaticamente quando esse resultado ocorre.
    public bool Verify(User user, string password) =>
        hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
}
