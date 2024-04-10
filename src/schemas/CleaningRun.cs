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
/// <param name="id">The unique identifier of the cleaning run. This is read-only and cannot be set externally.</param>
/// <param name="date">The date of the cleaning run.</param>
/// <param name="cleaners">The users who are assigned to the cleaning run.</param>
/// <param name="cleanerIds">The unique identifiers of the users who are assigned to the cleaning run.</param>
/// <param name="startingRoom">The room where the cleaning run starts.</param>
/// <param name="startingRoomId">The unique identifier of the room where the cleaning run starts.</param>
/// <param name="Phase">The phase of the cleaning run.</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[method: JsonConstructor]
public record CleaningRun([SwaggerSchema(ReadOnly = true)] Guid? id
						, DateTime? date
						, [SwaggerSchema(ReadOnly = true)] IEnumerable<User>? cleaners
						, [SwaggerSchema(WriteOnly = true)] IEnumerable<Guid>? cleanerIds
						, [SwaggerSchema(ReadOnly = true)] Room? startingRoom
						, [SwaggerSchema(WriteOnly = true)] Guid? startingRoomId
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
