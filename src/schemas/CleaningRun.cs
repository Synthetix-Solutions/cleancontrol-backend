#region

using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record CleaningRun([SwaggerSchema(ReadOnly = true)] Guid id, DateTime date, CleaningTeam cleaningTeam, Room startingRoom);
