using System.ComponentModel.DataAnnotations;

namespace SampleJwtAuth.Infrastructure.Authentication;

// Representa a seção Jwt da configuração; os atributos são validados na inicialização da API.
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    [Required] public string Issuer { get; init; } = "";
    [Required] public string Audience { get; init; } = "";
    [Required, MinLength(32)] public string SigningKey { get; init; } = "";
    [Range(1, 60)] public int ExpirationMinutes { get; init; } = 30;
}
