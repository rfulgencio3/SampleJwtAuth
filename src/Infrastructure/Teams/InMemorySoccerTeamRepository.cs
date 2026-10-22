using SampleJwtAuth.Application.Teams;
using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Infrastructure.Teams;

public sealed class InMemorySoccerTeamRepository : ISoccerTeamRepository
{
    // Estes dados são fixos no código, sem consulta ao banco; Cruzeiro aparece primeiro.
    private static readonly IReadOnlyList<SoccerTeam> Teams = Array.AsReadOnly<SoccerTeam>(
    [
        new(1, "Cruzeiro", "CRU", "Belo Horizonte", "MG"),
        new(2, "São Paulo", "SPFC", "São Paulo", "SP"),
        new(3, "Palmeiras", "PAL", "São Paulo", "SP"),
        new(4, "Flamengo", "FLA", "Rio de Janeiro", "RJ"),
        new(5, "Corinthians", "COR", "São Paulo", "SP"),
        new(6, "Santos", "SAN", "Santos", "SP"),
        new(7, "Atlético Mineiro", "CAM", "Belo Horizonte", "MG"),
        new(8, "Grêmio", "GRE", "Porto Alegre", "RS"),
        new(9, "Internacional", "INT", "Porto Alegre", "RS"),
        new(10, "Botafogo", "BOT", "Rio de Janeiro", "RJ"),
        new(11, "Fluminense", "FLU", "Rio de Janeiro", "RJ"),
        new(12, "Vasco da Gama", "VAS", "Rio de Janeiro", "RJ")
    ]);

    public IReadOnlyList<SoccerTeam> GetAll() => Teams;
}
