#region

using System.Collections.Immutable;
using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
///     Handlers for /users
/// </summary>
public static class Users {
	/// <summary>
	///     Deletes a user by its ID.
	/// </summary>
	/// <param name="userId">The unique identifier of the user to be deleted.</param>
	/// <param name="db">The database context.</param>
	/// <param name="userManager">The user manager.</param>
	/// <returns>
	///     <see cref="Ok" /> if the user was successfully deleted, else a <see cref="ProblemHttpResult" /> containing error
	///     details.
	///     If the user is not found, it returns <see cref="NotFound" />.
	/// </returns>
	public static Results<Ok, ProblemHttpResult, NotFound> DeleteUser(Guid userId
																	, CleancontrolContext db
																	, UserManager<CleanControlUser> userManager
	) {
		var dbUser = db.Users.Find(userId);
		if (dbUser is null)
			return TypedResults.Problem($"User with ID {userId} not found", statusCode: StatusCodes.Status404NotFound);

		userManager
		   .DeleteAsync(dbUser)
		   .Wait();

		db.SaveChanges();
		return TypedResults.Ok();
	}

	/// <summary>
	///     Updates a user by its ID
	/// </summary>
	/// <param name="userId">ID of the User</param>
	/// <param name="user">New data for the User</param>
	/// <param name="db"></param>
	/// <param name="userManager"></param>
	/// <returns><see cref="Ok" /> if user got updated, else a <see cref="ProblemHttpResult" /> containing error details.</returns>
	public static async Task<Results<Ok<User>, ProblemHttpResult, NotFound>> UpdateUser(Guid userId
																					  , User user
																					  , CleancontrolContext db
																					  , UserManager<CleanControlUser> userManager
	) {
		var dbUser = await db.Users.FindAsync(userId);
		if (dbUser is null)
			return TypedResults.Problem($"User with ID {userId} not found", statusCode: StatusCodes.Status404NotFound);

		dbUser.IsAdUser = user.isAdUser!.Value;
		dbUser.Name = user.name;

		var currentRoles = await userManager.GetRolesAsync(dbUser);
		var newRole = user.role.ToString()!;

		await userManager.RemoveFromRolesAsync(dbUser, currentRoles);
		await userManager.AddToRoleAsync(dbUser, newRole);

		await db.SaveChangesAsync();

		var role = Enum.Parse<Role>((await userManager.GetRolesAsync(dbUser)).First());

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
	///     Gets a user by its ID
	/// </summary>
	/// <param name="userId">ID of the user</param>
	/// <param name="db"></param>
	/// <param name="userManager"></param>
	/// <param name="context"></param>
	/// <returns><see cref="Ok{User}" /> with the user data, else a <see cref="ProblemHttpResult" /> containing error details.</returns>
	public static async Task<Results<Ok<User>, ProblemHttpResult, NotFound, ForbidHttpResult>> GetUser(
		Guid userId
	  , CleancontrolContext db
	  , UserManager<CleanControlUser> userManager
	  , HttpContext context
	) =>
		await GetUserFromDB(
							userId
						  , db
						  , userManager
						  , context
						   );

	private static async Task<Results<Ok<User>, ProblemHttpResult, NotFound, ForbidHttpResult>> GetUserFromDB(
		Guid userId
	  , CleancontrolContext db
	  , UserManager<CleanControlUser> userManager
	  , HttpContext context
	) {
		var dbUser = await db.Users.FindAsync(userId);
		if (dbUser is null)
			return TypedResults.Problem($"User with ID {userId} not found", statusCode: StatusCodes.Status404NotFound);

		var currentUser = await userManager.GetUserAsync(context.User);

		if (currentUser!.Id != dbUser.Id && await userManager.IsInRoleAsync(currentUser, Role.Admin.ToString()))
			return TypedResults.Forbid();

		var user = await User.FromDbUser(userManager, dbUser);
		return TypedResults.Ok(user);
	}

	/// <summary>
	///     Gets the current logged in user.
	/// </summary>
	/// <param name="db">The database context.</param>
	/// <param name="userManager">The user manager.</param>
	/// <param name="context">The HTTP context.</param>
	/// <returns>
	///     <see cref="Ok{User}" /> with the current user data, else a <see cref="ProblemHttpResult" /> containing error
	///     details.
	///     If the user is not found, it returns <see cref="NotFound" />.
	///     If the user is not authorized to perform the operation, it returns <see cref="ForbidHttpResult" />.
	/// </returns>
	public static async Task<Results<Ok<User>, ProblemHttpResult, NotFound, ForbidHttpResult>> GetCurrentUser(
		CleancontrolContext db
	  , UserManager<CleanControlUser> userManager
	  , HttpContext context
	) =>
		await GetUserFromDB(
							(await userManager.GetUserAsync(context.User))!.Id
						  , db
						  , userManager
						  , context
						   );


	/// <summary>
	///     Gets all users.
	/// </summary>
	/// <param name="db"></param>
	/// <param name="userManager"></param>
	/// <returns><see cref="Ok{IEnumerable}" /> containing all user data. Passwords are omitted.</returns>
	public static Ok<IEnumerable<User>> GetAllUsers(CleancontrolContext db, UserManager<CleanControlUser> userManager) {
		var dbUsers = db.Users;
		var users = dbUsers
				   .ToImmutableArray()
				   .Select(
						   u => new User(
										 u.Id
									   , u.Name
									   , u.Email
									   , u.GetRole(userManager)
										  .Result
									   , u.IsAdUser
										)
						  );
		return TypedResults.Ok(users.AsEnumerable());
	}

	/// <summary>
	///     Creates a new user.
	/// </summary>
	/// <param name="user">The user data.</param>
	/// <param name="db">The database context.</param>
	/// <param name="userManager">The user manager.</param>
	/// <returns>
	///     <see cref="CreatedAtRoute{User}" /> with the created user data, else a <see cref="Ok" />.
	///     If the user creation fails, it returns a <see cref="ProblemHttpResult" /> containing error details.
	/// </returns>
	public static async Task<Results<CreatedAtRoute<User>, Ok>> CreateUser(User user
																		 , CleancontrolContext db
																		 , UserManager<CleanControlUser> userManager
	) {
		var dbUser = new CleanControlUser {
			UserName = user.email
		  , Email = user.email
		  , Name = user.name
		  , IsAdUser = user.isAdUser!.Value
		};

		await userManager.CreateAsync(dbUser, user.password!);

		var role = user.role.ToString()!;
		await userManager.AddToRoleAsync(dbUser, role);
		var newRole = Enum.Parse<Role>((await userManager.GetRolesAsync(dbUser)).First());

		var returnUser = new User(
								  dbUser.Id
								, dbUser.Name
								, dbUser.Email
								, newRole
								, dbUser.IsAdUser
								 );

		return TypedResults.CreatedAtRoute(
										   returnUser
										 , "GetUserById"
										 , new { userId = dbUser.Id }
										  );
	}
}
