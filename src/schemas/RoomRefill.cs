
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanControlBackend.Schemas;

public record RoomRefill([SwaggerSchema(ReadOnly = true)] Product item, [SwaggerSchema(WriteOnly = true)] Guid? id,  [Required] int quantity);
