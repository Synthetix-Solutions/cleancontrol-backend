using Swashbuckle.AspNetCore.Annotations;
namespace CleanControlBackend.Schemas;

public record Product([SwaggerSchema(ReadOnly = true)] Guid id, string name, int inventoryQuantity);
