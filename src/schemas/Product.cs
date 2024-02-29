#region

using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record Product([SwaggerSchema(ReadOnly = true)] Guid id, string name, int inventoryQuantity, byte[]? image) {
	public Product(Guid id, string name, int inventoryQuantity) : this(id, name, inventoryQuantity, null) { }
};
