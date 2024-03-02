#region

using System.Security.Claims;
using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
/// Handlers for /users
/// </summary>
public static class Users {
	/// <summary>
	/// Deletes a user by its ID
	/// </summary>
	/// <param name="id">ID of the User</param>
	/// <param name="db"></param>
	/// <returns><see cref="Ok"/> if user got deleted, else a <see cref="ProblemHttpResult"/> containing error details.</returns>
	public static Results<Ok, ProblemHttpResult, NotFound> DeleteUser(string id, CleancontrolContext db,UserManager<CleanControlUser> userManager) {
		var dbUser = db.Users.Find(id);
		if (dbUser is null)
			return TypedResults.Problem($"User with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		userManager.DeleteAsync(dbUser).Wait();

		db.SaveChanges();
		return TypedResults.Ok();
	}

	/// <summary>
	/// Updates a user by its ID
	/// </summary>
	/// <param name="id">ID of the User</param>
	/// <param name="user">New data for the User</param>
	/// <param name="db"></param>
	/// <param name="userManager"></param>
	/// <returns><see cref="Ok"/> if user got updated, else a <see cref="ProblemHttpResult"/> containing error details.</returns>
	public static async Task<Results<Ok<User>, ProblemHttpResult, NotFound>> UpdateUser(string id
																					  , User user
																					  , CleancontrolContext db
																					  , UserManager<CleanControlUser> userManager
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

		var role = await GetRoleForUser(dbUser,userManager);

		var returnUser = new User(
								  dbUser.Id
								, dbUser.Name
								, dbUser.Email!
								, role
								, dbUser.IsAdUser
								 );
		return TypedResults.Ok(returnUser);
	}

	/// <summary>
	/// Gets the role for a user.
	/// </summary>
	/// <param name="dbUser">User, for which the role should be returned.</param>
	/// <param name="userManager"></param>
	/// <returns>Role of the user.</returns>
	private static async Task<Role> GetRoleForUser(CleanControlUser dbUser,UserManager<CleanControlUser> userManager) {
		var role = (await userManager.GetClaimsAsync(dbUser)).First(c => c.Type == ClaimTypes.Role)
															 .Value;
		return Enum.Parse<Role>(role);
	}

	/// <summary>
	/// Gets a user by its ID
	/// </summary>
	/// <param name="id">ID of the user</param>
	/// <param name="db"></param>
	/// <param name="userManager"></param>
	/// <returns><see cref="Ok{User}"/> with the user data, else a <see cref="ProblemHttpResult"/> containing error details.</returns>
	public static async Task<Results<Ok<User>, ProblemHttpResult ,NotFound>> GetUser(string id, CleancontrolContext db, UserManager<CleanControlUser> userManager) {
		var dbUser = await db.Users.FindAsync(id);
		if (dbUser is null)
			return TypedResults.Problem($"User with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		var user = new User(
							dbUser.Id
						  , dbUser.Name
						  , dbUser.Email
						  , await GetRoleForUser( dbUser, userManager)
						  , dbUser.IsAdUser
						   );
		return TypedResults.Ok(user);
	}

	/// <summary>
	/// Gets all users.
	/// </summary>
	/// <param name="db"></param>
	/// <param name="userManager"></param>
	/// <returns><see cref="Ok{IEnumerable}"/> containing all user data. Passwords are omitted.</returns>
	public static Ok<IEnumerable<User>> GetAllUsers(CleancontrolContext db, UserManager<CleanControlUser> userManager) {
		var dbUsers = db.Users;
		var users = dbUsers.Select(
								   u => new User(
												 u.Id
											   , u.Name
											   , u.Email
											   , GetRoleForUser(u, userManager)
													.Result
											   , u.IsAdUser
												)
								  );
		return TypedResults.Ok(users.AsEnumerable());
	}
}
