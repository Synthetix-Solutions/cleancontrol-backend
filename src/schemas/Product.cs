#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a product DTO.
/// </summary>
/// <param name="id">The unique identifier of the product. This is read-only.</param>
/// <param name="name">The name of the product.</param>
/// <param name="inventoryQuantity">The quantity of the product in inventory.</param>
/// <param name="image">The image of the product. This can be null.</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Product([SwaggerSchema(ReadOnly = true)] Guid? id, [Required] string name, [Required] int inventoryQuantity, string? image) {
	/// <inheritdoc />
	[JsonConstructor]
	public Product(string name, int inventoryQuantity, string image) : this(
																			null
																		  , name
																		  , inventoryQuantity
																		  , image
																		   ) { }

	/// <inheritdoc />
	public Product(Guid id, string name, int inventoryQuantity) : this(
																	   id
																	 , name
																	 , inventoryQuantity
																	 , null
																	  ) { }
}
