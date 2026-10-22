using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleJwtAuth.Application.Authentication;

namespace SampleJwtAuth.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthenticationService authenticationService,
    RegisterUser registerUser) : ControllerBase
{
    // O cadastro é público: o aluno pode criar uma conta antes de obter seu primeiro token.
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<UserResponse> Register(RegisterUserRequest request)
    {
        var user = registerUser.Execute(request.Username, request.Password);
        if (user is null)
            return Conflict(new ProblemDetails { Status = 409, Title = "Username already exists." });

        return StatusCode(StatusCodes.Status201Created, user);
    }

    // O login é público, mas só retorna um token se o usuário existir e a senha estiver correta.
    [AllowAnonymous]
    [HttpPost("token")]
    public ActionResult<TokenResponse> CreateToken(CreateTokenRequest request)
    {
        // Orienta clientes e intermediários a não armazenarem a resposta que contém o token em cache.
        Response.Headers.CacheControl = "no-store";
        var token = authenticationService.Authenticate(request.Username, request.Password);
        return token is null ? Unauthorized() : Ok(token);
    }
}

// Com ApiController, campos inválidos geram 400 antes de executar o método do endpoint.
public sealed record CreateTokenRequest(
    [Required, StringLength(100)] string Username,
    [Required, StringLength(256)] string Password);

public sealed record RegisterUserRequest(
    [Required, StringLength(100, MinimumLength = 3)] string Username,
    [Required, StringLength(256, MinimumLength = 8)] string Password);
