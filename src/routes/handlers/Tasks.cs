#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

#endregion

namespace CleanControlBackend.Routes.Handlers;

public static class Tasks {
	public static Results<Ok, ProblemHttpResult> DeleteTask(Guid taskId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		db.CleaningTasks.Remove(dbTask);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	public static Results<Ok<CleaningTask>, ProblemHttpResult> UpdateTask(Guid taskId, CleaningTask task, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		(_, dbTask.Name, dbTask.Description, dbTask.RecurrenceInterval, dbTask.OnCheckout) = task;

		db.SaveChanges();
		return TypedResults.Ok<CleaningTask>(null);
	}

	public static Results<Ok<CleaningTask>, ProblemHttpResult> GetTask(Guid id, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(id);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		var returnTask = new CleaningTask(
										  dbTask.Id
										, dbTask.Name
										, dbTask.Description
										, dbTask.RecurrenceInterval
										, dbTask.OnCheckout
										 );

		return TypedResults.Ok(returnTask);
	}

	public static Ok<CleaningTask> AddTask(CleaningTask task, CleancontrolContext db) {
		var dbTask = new CleanControlDb.CleaningTask {
			Name = task.name
		  , Description = task.description
		  , RecurrenceInterval = task.recurrenceInterval
		  , OnCheckout = task.onCheckout
		};

		var returnTasks = CreateReturnTask(dbTask);
		return TypedResults.Ok(returnTasks);
	}

	public static Ok<IEnumerable<CleaningTask>> GetAllTasks(CleancontrolContext db) => TypedResults.Ok(db.CleaningTasks.Select(CreateReturnTask));

	public static CleaningTask CreateReturnTask(CleanControlDb.CleaningTask dbTask) =>
		new(
			dbTask.Id
		  , dbTask.Name
		  , dbTask.Description
		  , dbTask.RecurrenceInterval
		  , dbTask.OnCheckout
		   );
}
