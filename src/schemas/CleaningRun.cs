#region

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CleanControlDb;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a cleaning run DTO.
/// </summary>
/// <param name="id">The unique identifier of the cleaning run</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[method: JsonConstructor]
public record CleaningRun([SwaggerSchema(ReadOnly = true)] Guid? id
						, DateTime? date
						, IEnumerable<User>? cleaners
						, IEnumerable<Guid>? cleanerIds
						, Room? startingRoom
						, Guid? startingRoomId
						, CleaningRunPhase? Phase
) {
	/// <inheritdoc />
	public CleaningRun(Guid id, DateTime date, Room startingRoom, IEnumerable<User> cleaners, CleaningRunPhase phase) : this(
																															 id
																														   , date
																														   , cleaners
																														   , null
																														   , startingRoom
																														   , null
																														   , phase
																															) { }
}
