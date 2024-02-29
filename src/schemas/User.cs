#region

using System.ComponentModel.DataAnnotations;
using CleanControlDb;
using Swashbuckle.AspNetCore.Annotations;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a User in the system.
/// </summary>
/// <param name="id">A unique identifier for the User.</param>
/// <param name="name">The name of the User.</param>
/// <param name="username">The username of the User.</param>
/// <param name="role">The role of the User in the system.</param>
/// <param name="password">Password of the User</param>
/// <param name="isAdUser"><see langword="true" />, if User is an AD user</param>
public record User(Guid? id
				 , [Required] string name
				 , [Required] string? username
				 , [Required] Role? role
				 , [SwaggerSchema(WriteOnly = true)] string? password
				 , [Required] bool? isAdUser
) {
	public User(Guid id, string name, string username) : this(id, name, username, null, null, null) { }
};

/// <summary>
///     Represents a User's login credentials.
/// </summary>
/// <param name="username">The username of the User, used for login.</param>
/// <param name="password">The password of the User, used for login.</param>
public record UserLogin(string username, string password);
