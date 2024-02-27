using Swashbuckle.AspNetCore.Annotations;

namespace CleanControlBackend.Schemas;

public record CleaningTask([SwaggerSchema(ReadOnly = true)] Guid Id, string name, string? Description, int? recurrenceInterval, bool onCheckout);
