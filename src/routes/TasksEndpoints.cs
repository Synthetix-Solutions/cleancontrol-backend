#region

using CleanControlDb;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

#endregion

namespace CleanControlBackend.Routes;

public static class TasksEndpoints {
	public static void Map(WebApplication app, CleancontrolContext db) {
		app.MapGroup("/tasks").MapTasksApi(db).WithOpenApi().WithTags("Tasks");
	}

	private static RouteGroupBuilder MapTasksApi(this RouteGroupBuilder group, CleancontrolContext db) {
		group
		   .MapGet("/", () => TypedResults.Ok<CleaningTask[]>(null))
		   .WithDescription("Fetches all tasks")
		   .WithSummary("Get all tasks");

		group
		   .MapPost("/", () => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Creates a new task")
		   .WithSummary("Create a new task");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Fetches a task by its ID")
		   .WithSummary("Get a task by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Updates a task by its ID")
		   .WithSummary("Update a task");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok())
		   .WithDescription("Deletes a task by its ID")
		   .WithSummary("Delete a task");

		return group;
	}
}
