using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleJwtAuth.Application.Teams;
using SampleJwtAuth.Domain;

namespace SampleJwtAuth.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/SoccerScoreTeams")]
public sealed class SoccerScoreTeamsController(GetSoccerScoreTeams getTeams) : ControllerBase
{
    // O middleware valida o JWT antes de executar este método; token inválido resulta em 401.
    [HttpGet]
    public ActionResult<IReadOnlyList<SoccerTeam>> Get() => Ok(getTeams.Execute());
}
