#region

using CleanControlBackend.Routes.Handlers;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

#endregion

namespace CleanControlBackend.Routes;

/// <summary>
///     Contains endpoints for /tasks
/// </summary>
public static class TasksEndpoints {
	/// <summary>
	///     Maps routes for /tasks
	/// </summary>
	/// <param name="app"></param>
	public static void Map(WebApplication app) {
		app
		   .MapGroup("/tasks")
		   .MapTasksApi()
		   .AddFluentValidationAutoValidation()
		   .WithOpenApi()
		   .WithTags("Tasks");
	}

	/// <summary>
	///     Maps /tasks endpoints
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	private static RouteGroupBuilder MapTasksApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", Tasks.GetAllTasks)
		   .WithDescription("Fetches all tasks")
		   .WithSummary("Get all tasks");

		group
		   .MapPost("/", Tasks.AddTask)
		   .WithDescription("Creates a new task")
		   .WithSummary("Create a new task");

		group
		   .MapGet("/{taskId:guid}", Tasks.GetTask)
		   .WithDescription("Fetches a task by its ID")
		   .WithSummary("Get a task by ID");

		group
		   .MapPut("/{taskId:guid}", Tasks.UpdateTask)
		   .WithDescription("Updates a task by its ID")
		   .WithSummary("Update a task");

		group
		   .MapDelete("/{taskId:guid}", Tasks.DeleteTask)
		   .WithDescription("Deletes a task by its ID")
		   .WithSummary("Delete a task");

		group
		   .MapPut("/{taskId:guid}/rooms", Tasks.AssignRooms)
		   .WithDescription("Sets the assigned rooms of a task")
		   .WithSummary("Sets the assigned rooms of a task");

		group
		   .MapGet("/{taskId:guid}/rooms", Tasks.GetAssignedRooms)
		   .WithDescription("Gets the assigned rooms of a task")
		   .WithSummary("Gets the assigned rooms of a task");

		return group;
	}
}
