#region

using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record CleaningTask([SwaggerSchema(ReadOnly = true)] Guid Id, string name, string? description, int? recurrenceInterval, bool onCheckout);
