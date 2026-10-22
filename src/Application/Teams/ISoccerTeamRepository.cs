using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Application.Teams;

public interface ISoccerTeamRepository
{
    IReadOnlyList<SoccerTeam> GetAll();
}
