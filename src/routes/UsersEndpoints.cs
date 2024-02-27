#region

using cleancontrol_db;
using CleanControlBackend.Logic;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using User = CleanControlBackend.Schemas.User;

#endregion

namespace CleanControlBackend.Routes;

public static class UsersEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/users").MapUserApi().WithOpenApi().WithTags("Users");
	}

	public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group) {
		// Implement your logic to fetch all users here
		group.MapGet("/", GetAllUsers).WithDescription("Fetches all users").WithSummary("Get all users");
		group.MapGet("/{id:guid}", GetUser).WithDescription("Fetches a user by its ID").WithSummary("Get a user by ID").WithName("GetUserById");
		group.MapPost("/", CreateUser).WithDescription("Creates a new user").WithSummary("Create a new user");
		group.MapPut("/{id:guid}", UpdateUser).WithDescription("Updates a user by its ID").WithSummary("Update a user");
		group.MapDelete("/{id:guid}", DeleteUser).WithDescription("Deletes a user by its ID").WithSummary("Delete a user");

		return group;
	}

	private static Results<Ok, NotFound> DeleteUser(Guid id, CleancontrolContext db) {
		var dbUser = db.Users.Find(id);
		if (dbUser is null) return TypedResults.NotFound();

		db.Users.Remove(dbUser);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	private static Results<Ok<User>, NotFound> UpdateUser(Guid id, User user, CleancontrolContext db) {
		var dbUser = db.Users.Find(id);
		if (dbUser is null) return TypedResults.NotFound();

		dbUser.Name = user.name;
		dbUser.Role = user.role!.Value;
		dbUser.IsAdUser = user.isAdUser!.Value;

		var returnUser = new User(
								  dbUser.Id
								, dbUser.Name
								, dbUser.Username
								, dbUser.Role
								, null
								, dbUser.IsAdUser
								 );

		db.SaveChanges();
		return TypedResults.Ok<User>(returnUser);
	}

	private static Results<CreatedAtRoute<User>, BadRequest> CreateUser(HttpContext context, LinkGenerator linkGenerator, User user, CleancontrolContext db) {
		var newUser = new CleanControlDb.User() {
													Name = user.name
												  , Username = user.username
												  , Role = user.role.Value
												  , Password = user.password
												  , Id = Guid.NewGuid()
												  , IsAdUser = user.isAdUser.Value
												};
		db.Users.Add(newUser);

		// Generate a link to the newly created user
		var customerLink = linkGenerator.GetUriByName(context, "GetUserById", new { id = newUser.Id });

		var returnUser = new User(
								  newUser.Id
								, newUser.Name
								, newUser.Username
								, newUser.Role
								, null
								, newUser.IsAdUser
								 );

		db.SaveChanges();
		return TypedResults.CreatedAtRoute<User>(returnUser, "GetUserById", returnUser.id);
	}

	private static Results<Ok<User>, NotFound> GetUser(Guid id, CleancontrolContext db) {
		var dbUser = db.Users.Find(id);
		if (dbUser is null) return TypedResults.NotFound();

		var user = new User(
							dbUser.Id
						  , dbUser.Name
						  , dbUser.Username
						  , dbUser.Role
						  , null
						  , dbUser.IsAdUser
						   );
		return TypedResults.Ok(user);
	}

	private static Ok<IEnumerable<User>> GetAllUsers(CleancontrolContext db) {
		var dbUsers = Users.GetAllUsers(db);
		var users = dbUsers.Select(
								   u => new User(
												 u.Id
											   , u.Name
											   , u.Username
											   , u.Role
											   , u.Password
											   , u.IsAdUser
												)
								  );
		return TypedResults.Ok(users);
	}
}
