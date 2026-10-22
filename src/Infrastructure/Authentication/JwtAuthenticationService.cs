using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleJwtAuth.Application.Authentication;

namespace SampleJwtAuth.Infrastructure.Authentication;

public sealed class JwtAuthenticationService(
    IOptions<JwtOptions> jwtOptions,
    IUserRepository users,
    IPasswordService passwords,
    TimeProvider timeProvider) : IAuthenticationService
{
    public TokenResponse? Authenticate(string username, string password)
    {
        // Busca o cadastro no banco e verifica a senha contra o hash; não compara senhas em texto puro.
        var user = users.FindByUsername(username);
        // O mesmo resultado evita distinguir, na resposta, usuário inexistente de senha incorreta.
        if (user is null || !passwords.Verify(user, password))
            return null;

        var options = jwtOptions.Value;
        // UTC evita diferenças de fuso horário; a duração vem da configuração Jwt.
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(options.ExpirationMinutes);
        // Assina com HMAC-SHA256 usando a chave do servidor. A assinatura permite detectar alterações;
        // ela não criptografa o conteúdo do token, por isso não incluímos a senha nas claims.
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), // sub identifica o usuário.
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // jti identifica este token.
                // iat representa a emissão em segundos desde 01/01/1970 (horário Unix).
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            ],
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        // Serializa no formato header.payload.signature e devolve a validade para o cliente.
        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiresAt);
    }
}
