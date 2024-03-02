#region

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Product([SwaggerSchema(ReadOnly = true)] Guid? id, string name, int inventoryQuantity, byte[]? image) {
	[JsonConstructor]
	public Product(string name, int inventoryQuantity, byte[]? image) : this(
																			 null
																		   , name
																		   , inventoryQuantity
																		   , image
																			) { }

	public Product(Guid id, string name, int inventoryQuantity) : this(
																	   id
																	 , name
																	 , inventoryQuantity
																	 , null
																	  ) { }
}
