using Swashbuckle.AspNetCore.Annotations;
namespace CleanControlBackend.Schemas;

public record Room([SwaggerSchema(ReadOnly = true)] Guid id, string roomNumber);
