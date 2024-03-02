#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record RoomRefill([SwaggerSchema(ReadOnly = true)] Product item, [SwaggerSchema(WriteOnly = true)] Guid? id, [Required] int quantity) {
	public RoomRefill(Product item, int quantity) : this(
														 item
													   , null
													   , quantity
														) { }
}
