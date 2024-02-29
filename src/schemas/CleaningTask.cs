#region

using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

public record CleaningTask([SwaggerSchema(ReadOnly = true)] Guid Id, string name, string? description, int? recurrenceInterval, bool onCheckout);
