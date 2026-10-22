namespace SampleJwtAuth.Domain;

// A entidade descreve o time sem depender de HTTP, banco de dados ou autenticação.
public sealed record SoccerTeam(int Id, string Name, string Abbreviation, string City, string State);
