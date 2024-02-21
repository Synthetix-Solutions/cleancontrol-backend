#region

using cleancontrol_db;
using CleanControlBackend.Logic;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using User = CleanControlBackend.Schemas.User;

#endregion

namespace CleanControlBackend.Routes;

public static class UsersEndpoints {
	public static void Map(WebApplication app, CleancontrolContext db) {
		app.MapGroup("/users").MapUserApi(db).WithOpenApi().WithTags("Users");
	}

	public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group, CleancontrolContext db) {
		// Implement your logic to fetch all users here
		group
		   .MapGet(
				   "/"
				 , () => {
					   var dbUsers = Users.GetAllUsers(db);
					   var users = dbUsers.Select(u => new User(u.Id, u.Name, u.Username, u.Role));
					   return TypedResults.Ok(users);
				   }
				  )
		   .WithDescription("Fetches all users")
		   .WithSummary("Get all users");

		group
		   .MapPost("/",Results<Created<User>, BadRequest> () => {
							return TypedResults.Created<Schemas.User>("",null);
						})
		   .WithDescription("Creates a new user")
		   .WithSummary("Create a new user");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<User>(null))
		   .WithDescription("Fetches a user by its ID")
		   .WithSummary("Get a user by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<User>(null))
		   .WithDescription("Updates a user by its ID")
		   .WithSummary("Update a user");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok())
		   .WithDescription("Deletes a user by its ID")
		   .WithSummary("Delete a user");

		return group;
	}
}
