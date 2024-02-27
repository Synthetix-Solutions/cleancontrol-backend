#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

#endregion

namespace CleanControlBackend.Routes;

public static class TasksEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/tasks").MapTasksApi().WithOpenApi().WithTags("Tasks");
	}

	private static RouteGroupBuilder MapTasksApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", GetAllTasks)
		   .WithDescription("Fetches all tasks")
		   .WithSummary("Get all tasks");

		group
		   .MapPost("/", AddTask)
		   .WithDescription("Creates a new task")
		   .WithSummary("Create a new task");

		group
		   .MapGet("/{id}", GetTask)
		   .WithDescription("Fetches a task by its ID")
		   .WithSummary("Get a task by ID");

		group
		   .MapPut("/{id}", UpdateTask)
		   .WithDescription("Updates a task by its ID")
		   .WithSummary("Update a task");

		group
		   .MapDelete("/{id}", DeleteTask)
		   .WithDescription("Deletes a task by its ID")
		   .WithSummary("Delete a task");

		return group;
	}

	private static Ok DeleteTask(Guid id) {
		return TypedResults.Ok();
	}

	private static Ok<CleaningTask> UpdateTask(Guid id) {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<CleaningTask> GetTask(Guid id) {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<CleaningTask> AddTask() {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<CleaningTask[]> GetAllTasks() {
		return TypedResults.Ok<CleaningTask[]>(null);
	}
}
