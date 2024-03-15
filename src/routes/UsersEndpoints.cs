#region

using CleanControlBackend.Routes.Handlers;
using CleanControlBackend.Schemas;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

#endregion

namespace CleanControlBackend.Routes;

/// <summary>
/// Contains endpoints for /users
/// </summary>
public static class UsersEndpoints {
	/// <summary>
	/// Maps routes for /users
	/// </summary>
	/// <param name="app"></param>
	public static void Map(WebApplication app) {
		app
		   .MapGroup("/users")
		   // .RequireAuthorization(Policies.AdminOrCleanerOnly)
		   .MapUserApi()		   .AddFluentValidationAutoValidation()
		   .WithOpenApi()
		   .WithTags("Users");
	}

	/// <summary>
	/// Maps /users
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	public static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group) {
		// Implement your logic to fetch all users here
		group
		   .MapGet("/", Users.GetAllUsers)
		   .RequireAuthorization(Policies.AdminOrCleanerOnly)
		   .WithDescription("Fetches all users")
		   .WithSummary("Get all users");
		group
		   .MapGet("/{userId:guid}", Users.GetUser)
		   .WithDescription("Fetches a user by its ID")
		   .WithSummary("Get a user by ID")
		   .WithName("GetUserById");
		group
		   .MapPut("/{userId:guid}", Users.UpdateUser)
		   .RequireAuthorization(Policies.AdminOnly)
		   .WithDescription("Updates a user by its ID")
		   .WithSummary("Update a user");
		group
		   .MapPost("", Users.CreateUser)
		   .WithDescription("Updates a user by its ID")
		   .WithSummary("Update a user");
		group
		   .MapDelete("/{userId:guid}", Users.DeleteUser)
		   .RequireAuthorization(Policies.AdminOnly)
		   .WithDescription("Deletes a user by its ID")
		   .WithSummary("Delete a user");
		group
		   .MapGet("/me", Users.GetCurrentUser)
		   .RequireAuthorization(Policies.AdminOrCleanerOnly)
		   .WithDescription("Deletes a user by its ID")
		   .WithSummary("Delete a user");
		return group;
	}
}
