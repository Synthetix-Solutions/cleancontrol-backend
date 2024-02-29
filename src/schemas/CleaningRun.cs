#region

using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record CleaningRun([SwaggerSchema(ReadOnly = true)] Guid id, DateTime date, IEnumerable<User>? cleaners, IEnumerable<Guid>? cleanerIds,  Room? startingRoom, Guid? startingRoomId) {
	public CleaningRun(Guid id, DateTime date, Room startingRoom, IEnumerable<User> cleaners) : this(id, date, cleaners, null, startingRoom, null) { }
};
