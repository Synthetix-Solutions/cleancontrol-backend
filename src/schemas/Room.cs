#region

using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record Room([SwaggerSchema(ReadOnly = true)] Guid id, string roomNumber);
