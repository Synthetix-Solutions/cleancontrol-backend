#region

using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record CleaningRun([SwaggerSchema(ReadOnly = true)] Guid id
						, DateTime date
						, IEnumerable<User>? cleaners
						, IEnumerable<Guid>? cleanerIds
						, Room? StartingRoom
						, Guid? startingRoomId
) {
	public CleaningRun(Guid id, DateTime date, Room StartingRoom, IEnumerable<User> cleaners) : this(
																									 id
																								   , date
																								   , cleaners
																								   , null
																								   , StartingRoom
																								   , null
																									) { }
}
