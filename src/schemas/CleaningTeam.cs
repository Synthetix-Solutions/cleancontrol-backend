#region

using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record CleaningTeam([SwaggerSchema(ReadOnly = true)] Guid id, string name, IEnumerable<User> cleaners);
