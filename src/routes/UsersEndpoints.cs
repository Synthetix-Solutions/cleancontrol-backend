#region

using System.Security.Claims;
using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using User = CleanControlBackend.Schemas.User;

#endregion

namespace CleanControlBackend.Routes;

public static class UsersEndpoints {
	public static void Map(WebApplication app) {
		app
		   .MapGroup("/users")
		   .RequireAuthorization(Policies.AdminOrCleanerOnly)
		   .MapUserApi()
		   .WithOpenApi()
		   .WithTags("Users");
	}

	public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group) {
		// Implement your logic to fetch all users here
		group
		   .MapGet("/", GetAllUsers)
		   .RequireAuthorization(Policies.AdminOnly)
		   .WithDescription("Fetches all users")
		   .WithSummary("Get all users");
		group
		   .MapGet("/{id}", GetUser)
		   .WithDescription("Fetches a user by its ID")
		   .WithSummary("Get a user by ID")
		   .WithName("GetUserById");
		group
		   .MapPut("/{id}", UpdateUser)
		   .RequireAuthorization(Policies.AdminOnly)
		   .WithDescription("Updates a user by its ID")
		   .WithSummary("Update a user");
		group
		   .MapDelete("/{id}", DeleteUser)
		   .RequireAuthorization(Policies.AdminOnly)
		   .WithDescription("Deletes a user by its ID")
		   .WithSummary("Delete a user");

		return group;
	}

	private static Results<Ok, NotFound> DeleteUser(string id, CleancontrolContext db) {
		var dbUser = db.Users.Find(id);
		if (dbUser is null)
			return TypedResults.NotFound();

		db.Users.Remove(dbUser);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	private static async Task<Results<Ok<User>, ProblemHttpResult, NotFound>> UpdateUser(string id
																					   , User user
																					   , CleancontrolContext db
																					   , UserManager<CleanControlUser> userManager
																					   , ClaimsPrincipal currentUser
	) {
		var dbUser = await db.Users.FindAsync(id);
		if (dbUser is null)
			return TypedResults.Problem($"User with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		dbUser.IsAdUser = user.isAdUser!.Value;
		dbUser.Name = user.name;

		var claim = new Claim(ClaimTypes.Role, user.role.ToString()!);
		await userManager.ReplaceClaimAsync(
											dbUser
										  , claim
										  , claim
										   );


		await db.SaveChangesAsync();

		var role = await GetRoleForUser(userManager, dbUser);

		var returnUser = new User(
								  dbUser.Id
								, dbUser.Name
								, dbUser.Email!
								, role
								, dbUser.IsAdUser
								 );
		return TypedResults.Ok(returnUser);
	}

	private static async Task<CleanControlDb.Role> GetRoleForUser(UserManager<CleanControlUser> userManager, CleanControlUser dbUser) {
		var role = (await userManager.GetClaimsAsync(dbUser)).First(c => c.Type == ClaimTypes.Role)
															 .Value;
		return Enum.Parse<CleanControlDb.Role>(role);
	}

	private static async Task<Results<Ok<User>, NotFound>> GetUser(string id, CleancontrolContext db, UserManager<CleanControlUser> userManager) {
		var dbUser = await db.Users.FindAsync(id);
		if (dbUser is null)
			return TypedResults.NotFound();

		var user = new User(
							dbUser.Id
						  , dbUser.Name
						  , dbUser.Email
						  , await GetRoleForUser(userManager, dbUser)
						  , dbUser.IsAdUser
						   );
		return TypedResults.Ok(user);
	}

	private static Ok<IEnumerable<User>> GetAllUsers(CleancontrolContext db,UserManager<CleanControlUser> userManager) {
		var dbUsers = db.Users;
		var users = dbUsers.Select(
								   u => new User(
												 u.Id
											   , u.Name
											   , u.Email
											   , GetRoleForUser(userManager, u).Result
											   , u.IsAdUser
												)
								  );
		return TypedResults.Ok(users.AsEnumerable());
	}
}
