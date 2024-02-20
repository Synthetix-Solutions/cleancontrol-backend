#region

using cleancontrol_db;

#endregion

namespace cleancontrol_backend.Schemas;

/// <summary>
/// Represents a User in the system.
/// </summary>
/// <param name="id">A unique identifier for the User.</param>
/// <param name="name">The name of the User.</param>
/// <param name="username">The username of the User.</param>
/// <param name="role">The role of the User in the system.</param>
public record User(Guid id, string name, string username, Role? role);

/// <summary>
/// Represents a User's login credentials.
/// </summary>
/// <param name="username">The username of the User, used for login.</param>
/// <param name="password">The password of the User, used for login.</param>

public record UserLogin(string username, string password);
