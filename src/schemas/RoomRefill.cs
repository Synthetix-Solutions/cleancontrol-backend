#region

using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record RoomRefill([SwaggerSchema(ReadOnly = true)] Product item, [SwaggerSchema(WriteOnly = true)] Guid? id, [Required] int quantity) {
	public RoomRefill(Product item, int quantity) : this(
														 item
													   , null
													   , quantity
														) { }
}
