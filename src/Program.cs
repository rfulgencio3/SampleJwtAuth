using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SampleJwtAuth.Application.Authentication;
using SampleJwtAuth.Application.Teams;
using SampleJwtAuth.Infrastructure.Authentication;
using SampleJwtAuth.Infrastructure.Teams;
using SampleJwtAuth.Infrastructure.Persistence;
using SampleJwtAuth.Domain;

var builder = WebApplication.CreateBuilder(args);

// Etapa 1: lê a seção Jwt e valida os campos obrigatórios antes de iniciar a API.
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();

// Etapa 2: registra as implementações que serão fornecidas às classes por injeção de dependência.
builder.Services.AddSingleton(TimeProvider.System);
// Scoped cria uma instância por requisição; Singleton reutiliza a instância durante a execução.
builder.Services.AddScoped<IAuthenticationService, JwtAuthenticationService>();
// O banco nomeado compartilha os usuários entre requisições, mas perde os dados ao encerrar a API.
builder.Services.AddDbContext<AuthDbContext>(options => options.UseInMemoryDatabase("SampleJwtAuth"));
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<RegisterUser>();
builder.Services.AddSingleton<ISoccerTeamRepository, InMemorySoccerTeamRepository>();
builder.Services.AddScoped<GetSoccerScoreTeams>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SampleJwtAuth",
        Version = "v1",
        Description = "Create an access token and retrieve Brazilian soccer teams."
    });
    // O Swagger acrescenta Bearer automaticamente; no botão Authorize, cole apenas o token.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste ONLY the accessToken returned by POST /api/auth/token (starts with eyJ). Do NOT include 'Bearer': Swagger UI adds it automatically."
    });
    options.OperationFilter<SampleJwtAuth.Presentation.Swagger.BearerSecurityOperationFilter>();
});

// Etapa 3: configura como o servidor verifica os JWTs recebidos nas rotas protegidas.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, configuration) =>
    {
        var jwt = configuration.Value;
        // Mantém os nomes originais das claims, como sub, sem convertê-los para outros nomes.
        bearer.MapInboundClaims = false;
        // Quando a autenticação é exigida e falha, devolve 401 com uma explicação em JSON.
        bearer.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers.WWWAuthenticate = "Bearer";
                var header = context.Request.Headers.Authorization.ToString();
                var detail = header.StartsWith("Bearer Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? "The Authorization header contains 'Bearer' twice. In Swagger Authorize, paste only the accessToken, without the Bearer prefix."
                    : "A valid, unexpired access token is required. Generate a token using POST /api/auth/token and send Authorization: Bearer <accessToken>.";
                await Results.Problem(statusCode: StatusCodes.Status401Unauthorized,
                    title: "Unauthorized", detail: detail).ExecuteAsync(context.HttpContext);
            }
        };
        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Confere quem emitiu o token.
            ValidateAudience = true, // Confere para qual destinatário ele foi emitido.
            ValidateLifetime = true, // Confere início e fim da validade.
            ValidateIssuerSigningKey = true, // Valida a chave de assinatura usada no token.
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256], // Aceita apenas o algoritmo HS256.
            ClockSkew = TimeSpan.Zero // Não acrescenta tempo de tolerância à validade do token.
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Disponibiliza a interface de testes do Swagger somente no ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleJwtAuth v1"));
}

// Etapa 4: autentica o usuário antes de verificar se o endpoint permite seu acesso.
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
