using Swashbuckle.AspNetCore.Annotations;
namespace CleanControlBackend.Schemas;

public record CleaningTeam([SwaggerSchema(ReadOnly = true)] Guid id, string name, IEnumerable<User> cleaners);
