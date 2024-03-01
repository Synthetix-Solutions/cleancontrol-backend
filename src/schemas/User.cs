#region

using System.ComponentModel.DataAnnotations;
using CleanControlDb;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
///     Represents a User in the system.
/// </summary>
/// <param name="id">A unique identifier for the User.</param>
/// <param name="name">The name of the User.</param>
/// <param name="username">The username of the User.</param>
/// <param name="role">The role of the User in the system.</param>
/// <param name="isAdUser"><see langword="true" />, if User is an AD user</param>
public record User(string? id, [Required] string name, [Required] string username, [Required] Role? role, [Required] bool? isAdUser) {
	public User(string id, string name, string username) : this(
																id
															  , name
															  , null
															  , null
															  , null
															   ) { }
}
