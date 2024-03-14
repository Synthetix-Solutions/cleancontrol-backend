#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CleanControlDb;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a User in the system.
/// </summary>
/// <param name="id">A unique identifier for the User.</param>
/// <param name="name">The name of the User.</param>
/// <param name="email">The username of the User.</param>
/// <param name="role">The role of the User in the system.</param>
/// <param name="isAdUser"><see langword="true" />, if User is an AD user</param>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[method: JsonConstructor]
public record User (Guid? id
								  , [Required] string name
								  , [Required] string email
								  , [Required] Role? role
								  , [Required] bool? isAdUser
								  , [SwaggerSchema(WriteOnly = true)] string? password
								   ) {

	public User(Guid id, string name, string email)
		: this(id, name, email, null, null, null) { }
	public User(Guid id, string name, string email, Role role, bool isAdUser)
		: this(id, name, email, role, isAdUser, null) { }
};
