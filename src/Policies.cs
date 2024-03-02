#region

using CleanControlDb;
using Microsoft.AspNetCore.Authorization;

#endregion

namespace CleanControlBackend.Schemas;

/// <summary>
/// Policy types for authorization
/// </summary>
public static class Policies {
	/// <summary>
	/// Only allow admin users
	/// </summary>
	public const string AdminOnly = "AdminOnly";
	/// <summary>
	/// Allow any admin or cleaner user
	/// </summary>
	public const string AdminOrCleanerOnly = "AdminOrCleanerOnly";

	/// <summary>
	/// Init policies
	/// </summary>
	/// <param name="o">Options builder</param>
	public static void AddPolicies(AuthorizationOptions o) {
		o.AddPolicy(AdminOnly, p => p.RequireClaim(ClaimTypes.Role, Role.Admin.ToString()));
		o.AddPolicy(
					AdminOrCleanerOnly
				  , p => p.RequireClaim(
										ClaimTypes.Role
									  , Role.Admin.ToString()
									  , Role.Cleaner.ToString()
									   )
				   );
	}
}
