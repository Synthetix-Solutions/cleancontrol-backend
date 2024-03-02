#region

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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
[SuppressMessage("ReSharper", "InconsistentNaming")]
public record User(string? id
				 , [Required] string name
				 , [Required] string? username = null
				 , [Required] Role? role = null
				 , [Required] bool? isAdUser = null
);
