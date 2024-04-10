#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CleanControlDb.Extensions;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a Room.
/// </summary>
/// <param name="id">The unique identifier of the room. This is read-only.</param>
/// <param name="roomNumber">The number of the room.</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Room([SwaggerSchema(ReadOnly = true)] Guid id, [Required] string roomNumber, [SwaggerSchema(ReadOnly = true)] bool occupied) {
	/// <summary>
	///     Creates a new instance of the Room record from a database Room object.
	/// </summary>
	/// <param name="room">The database Room object.</param>
	/// <returns>A new instance of the Room record.</returns>
	public static Room FromDbRoom(CleanControlDb.Room room) =>
		new(
			room.Id
		  , room.Number
		  , room.RoomOccupancy?.Checkout >= DateTime.UtcNow.ToDateOnly()
		   );
}
