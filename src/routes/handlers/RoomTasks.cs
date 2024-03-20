#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
/// Handlers for /rooms/{roomId}/tasks
/// </summary>
public static class RoomTasks {
	/// <summary>
	/// Deletes a specific room task from the database.
	/// </summary>
	/// <param name="roomId">The ID of the room associated with the task.</param>
	/// <param name="taskId">The ID of the task to delete.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method deletes a specific room task from the database. It does this by finding the room and the task in the database using the provided room ID and task ID.
	/// If the room or the task is not found, it returns a <see cref="ProblemHttpResult"/> with a 404 status code and an error message.
	/// If the room and the task are found, it creates a new room cleanup with the found room and task, adds it to the database, saves the changes, and then returns an <see cref="Ok"/> result.
	/// </remarks>
	/// <returns>A <see cref="Results{T1, T2}"/> object that contains either an <see cref="Ok"/> result if the task was successfully deleted, or a <see cref="ProblemHttpResult"/> with an error message if the room or the task was not found.</returns>
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

	/// <summary>
	/// Retrieves a specific room task from the database.
	/// </summary>
	/// <param name="taskId">The ID of the task to retrieve.</param>
	/// <param name="roomId">The ID of the room associated with the task.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method retrieves a specific room task from the database. It does this by finding the task in the database using the provided task ID.
	/// If the task is not found, it returns a <see cref="ProblemHttpResult"/> with a 404 status code and an error message.
	/// If the task is found, it returns an <see cref="Ok{T}"/> result with the task.
	/// </remarks>
	/// <returns>A <see cref="Results{T1, T2}"/> object that contains either an <see cref="Ok{T}"/> result with the task, or a <see cref="ProblemHttpResult"/> with an error message if the task was not found.</returns>
	public static Results<Ok<CleaningTask>, ProblemHttpResult> GetRoomTask(Guid taskId, Guid roomId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem("Cleaning task with ID '{taskId}' not found.", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok(CleaningTask.FromDbRoomCleaningTask(dbTask));
	}

	/// <summary>
	/// Retrieves all due room tasks from the database.
	/// </summary>
	/// <param name="roomId">The ID of the room associated with the tasks.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	/// This method retrieves all due room tasks from the database. It does this by finding the room in the database using the provided room ID,
	/// and then selecting all cleaning tasks for the room where the next due date is less than or equal to the current date.
	/// If the room is not found, it returns a <see cref="ProblemHttpResult"/> with a 404 status code and an error message.
	/// If the room is found and the tasks are successfully retrieved, it returns an <see cref="Ok{T}"/> result with the list of tasks.
	/// </remarks>
	/// <returns>A <see cref="Results{T1, T2}"/> object that contains either an <see cref="Ok{T}"/> result with the list of tasks, or a <see cref="ProblemHttpResult"/> with an error message if the room was not found.</returns>
	public static Results<Ok<IEnumerable<CleaningTask>>, ProblemHttpResult> GetDueRoomTasks(Guid roomId, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var dbTasks = room.CleaningTasks.Where(ct => ct.GetNextDueDate(room) <= DateOnly.FromDateTime(DateTime.UtcNow));
		var tasks = dbTasks.Select(CleaningTask.FromDbRoomCleaningTask);

		return TypedResults.Ok(tasks);
	}


}
