using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Application.Authentication;

public sealed record UserResponse(string Username);

public sealed class RegisterUser(IUserRepository users, IPasswordService passwords)
{
    public UserResponse? Execute(string username, string password)
    {
        // Mantém o nome para exibição e cria outra versão para comparação sem diferenciar maiúsculas.
        var user = new User
        {
            Username = username.Trim(),
            NormalizedUsername = username.Trim().ToUpperInvariant()
        };
        // Gera um hash com salt (valor aleatório que diferencia os hashes) antes de salvar.
        // A senha original não é armazenada nem devolvida na resposta.
        user.PasswordHash = passwords.Hash(user, password);
        // Retorna somente o nome; null informa ao controller que a conta já existe (HTTP 409).
        return users.TryAdd(user) ? new UserResponse(user.Username) : null;
    }
}
