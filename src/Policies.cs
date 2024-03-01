using Microsoft.AspNetCore.Authorization;

namespace CleanControlBackend.Schemas;

public static class Policies {
	public const string AdminOnly = "AdminOnly";
	public const string AdminOrCleanerOnly = "AdminOrCleanerOnly";

	public static void AddPolicies(AuthorizationOptions o) {
		o.AddPolicy(Policies.AdminOnly, p => p.RequireClaim(ClaimTypes.Role, CleanControlDb.Role.Admin.ToString()));
		o.AddPolicy(
					Policies.AdminOrCleanerOnly
				  , p => p.RequireClaim(
										ClaimTypes.Role
									  , CleanControlDb.Role.Admin.ToString()
									  , CleanControlDb.Role.Cleaner.ToString()
									   )
				   );
	}
}

