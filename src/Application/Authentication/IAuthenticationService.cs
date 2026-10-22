namespace SampleJwtAuth.Application.Authentication;

// Define o contrato de login sem depender de JWT ou Entity Framework; a infraestrutura o implementa.
public interface IAuthenticationService
{
    TokenResponse? Authenticate(string username, string password);
}
