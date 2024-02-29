#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

#endregion

namespace CleanControlBackend.Routes;

public static class TasksEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/tasks")
		   .MapTasksApi()
		   .WithOpenApi()
		   .WithTags("Tasks");
	}

	private static RouteGroupBuilder MapTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllTasks)
			 .WithDescription("Fetches all tasks")
			 .WithSummary("Get all tasks");

		group.MapPost("/", AddTask)
			 .WithDescription("Creates a new task")
			 .WithSummary("Create a new task");

		group.MapGet("/{id}", GetTask)
			 .WithDescription("Fetches a task by its ID")
			 .WithSummary("Get a task by ID");

		group.MapPut("/{id}", UpdateTask)
			 .WithDescription("Updates a task by its ID")
			 .WithSummary("Update a task");

		group.MapDelete("/{id}", DeleteTask)
			 .WithDescription("Deletes a task by its ID")
			 .WithSummary("Delete a task");

		return group;
	}

	private static Results<Ok, ProblemHttpResult> DeleteTask(Guid taskId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		db.CleaningTasks.Remove(dbTask);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	private static Results<Ok<CleaningTask>, ProblemHttpResult> UpdateTask(Guid taskId, CleaningTask task, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		(_, dbTask.Name, dbTask.Description, dbTask.RecurrenceInterval, dbTask.OnCheckout) = task;

		db.SaveChanges();
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Results<Ok<CleaningTask>, ProblemHttpResult> GetTask(Guid id, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(id);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		var returnTask = new CleaningTask(dbTask.Id, dbTask.Name, dbTask.Description, dbTask.RecurrenceInterval, dbTask.OnCheckout);

		return TypedResults.Ok(returnTask);
	}

	private static Ok<CleaningTask> AddTask(CleaningTask task, CleancontrolContext db) {
		var dbTask = new CleanControlDb.CleaningTask() { Name = task.name, Description = task.description, RecurrenceInterval = task.recurrenceInterval, OnCheckout = task.onCheckout };

		var returnTasks = CreateReturnTask(dbTask);
		return TypedResults.Ok(returnTasks);
	}

	private static Ok<IEnumerable<CleaningTask>> GetAllTasks(CleancontrolContext db) => TypedResults.Ok( db.CleaningTasks.Select(CreateReturnTask));
	private static CleaningTask CreateReturnTask(CleanControlDb.CleaningTask dbTask) => new(dbTask.Id, dbTask.Name, dbTask.Description, dbTask.RecurrenceInterval, dbTask.OnCheckout);

}
