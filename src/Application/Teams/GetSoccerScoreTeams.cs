using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Application.Teams;

// O caso de uso depende de uma interface, sem precisar saber onde os times estão armazenados.
public sealed class GetSoccerScoreTeams(ISoccerTeamRepository repository)
{
    public IReadOnlyList<SoccerTeam> Execute() => repository.GetAll();
}
