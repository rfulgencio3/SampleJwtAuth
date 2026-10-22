namespace SampleJwtAuth.Application.Authentication;

// Dados de saída do login: o cliente recebe o token e a expiração, sem acesso à chave de assinatura.
public sealed record TokenResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);
