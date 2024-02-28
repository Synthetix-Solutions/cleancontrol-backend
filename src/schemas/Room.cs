#region

using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record Room([SwaggerSchema(ReadOnly = true)] Guid id, string roomNumber);
