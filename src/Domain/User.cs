namespace SampleJwtAuth.Domain;

public sealed class User
{
    // Versão sem espaços nas extremidades e em maiúsculas, usada para buscar e identificar a conta.
    public string NormalizedUsername { get; init; } = "";
    public string Username { get; init; } = "";
    // Contém apenas o hash produzido pelo serviço de senha, nunca a senha original.
    public string PasswordHash { get; set; } = "";
}
