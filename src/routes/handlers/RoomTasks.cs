using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

namespace CleanControlBackend.Routes.Handlers;

public static class RoomTasks {
	public static Results<Ok, ProblemHttpResult> DeleteRoomTask(Guid roomId, Guid taskId, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var cleaningTask = db.CleaningTasks.Find(taskId);
		if (cleaningTask is null)
			return TypedResults.Problem($"Cleaning task with ID '{taskId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var roomCleanup = new RoomCleanup { CleaningTask = cleaningTask, Room = room, Date = DateTime.UtcNow };
		db.RoomCleanups.Add(roomCleanup);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	public static Results<Ok<CleaningTask>, ProblemHttpResult> GetRoomTask(Guid taskId, Guid roomId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem("Cleaning task with ID '{taskId}' not found.", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok<CleaningTask>(CreateReturnRoomCleaningTask(dbTask));
	}

	public static Results<Ok<IEnumerable<CleaningTask>>, ProblemHttpResult> GetDueRoomTasks(Guid roomId, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var dbTasks = room.CleaningTasks.Where(ct => ct.GetNextDueDate(room) <= DateOnly.FromDateTime(DateTime.UtcNow));
		var tasks = dbTasks.Select(CreateReturnRoomCleaningTask);

		return TypedResults.Ok<IEnumerable<CleaningTask>>(tasks);
	}

	public static CleaningTask CreateReturnRoomCleaningTask(CleanControlDb.CleaningTask dbTask) =>
		new(
			dbTask.Id
		  , dbTask.Name
		  , dbTask.Description
		  , dbTask.RecurrenceInterval
		  , dbTask.OnCheckout
		   );
}