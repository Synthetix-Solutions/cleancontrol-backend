#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a RoomRefill.
/// </summary>
/// <param name="item">The product item to be refilled in the room. This is read-only.</param>
/// <param name="id">The unique identifier of the room refill. This is write-only and can be null.</param>
/// <param name="quantity">The quantity of the product item to be refilled in the room. This is required.</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record RoomRefill([SwaggerSchema(ReadOnly = true)] Product item, [SwaggerSchema(WriteOnly = true)] Guid? id, [Required] int quantity) {
	/// <inheritdoc />
	public RoomRefill(Product item, int quantity) : this(
														 item
													   , null
													   , quantity
														) { }
}
