#region

using System.Collections.Immutable;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
///     Handlers for /tasks
/// </summary>
public static class Tasks {
	/// <summary>
	///     Deletes a specific task from the database.
	/// </summary>
	/// <param name="taskId">The ID of the task to delete.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method deletes a specific task from the database. It does this by finding the task in the database using the
	///     provided task ID.
	///     If the task is not found, it returns a <see cref="ProblemHttpResult" /> with a message indicating that the task was
	///     not found and a 404 status code.
	///     If the task is found, it removes the task from the database, saves the changes, and then returns an
	///     <see cref="Ok" /> result.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok" /> result if the task was
	///     successfully deleted, or a <see cref="ProblemHttpResult" /> if the task was not found.
	/// </returns>
	public static Results<Ok, ProblemHttpResult> DeleteTask(Guid taskId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		db.CleaningTasks.Remove(dbTask);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	/// <summary>
	///     Assigns rooms to a specific task in the database.
	/// </summary>
	/// <param name="taskId">The ID of the task to which rooms are to be assigned.</param>
	/// <param name="roomIds">The IDs of the rooms to be assigned to the task.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method assigns rooms to a specific task in the database. It does this by finding the task and the rooms in the
	///     database using the provided task ID and room IDs.
	///     If the task or any of the rooms are not found, it returns a <see cref="ProblemHttpResult" /> with a message
	///     indicating that the task or the rooms were not found and a 404 status code.
	///     If the task and all the rooms are found, it assigns the rooms to the task and returns an <see cref="Ok" /> result.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok" /> result if the rooms were
	///     successfully assigned to the task, or a <see cref="ProblemHttpResult" /> if the task or any of the rooms were not
	///     found.
	/// </returns>
	public static Results<Ok, ProblemHttpResult> AssignRooms(Guid taskId, [FromBody] IEnumerable<Guid> roomIds, CleancontrolContext db) {
		var task = db.CleaningTasks.Find(taskId);
		var rooms = roomIds
				   .Select(id => db.Rooms.Find(id))
				   .ToList();

		var assignRooms = (CleanControlDb.CleaningTask t, ICollection<Room> r) => {
							  var ret = AssignRoomsToTask(t, r);
							  db.SaveChanges();
							  return ret;
						  };

		return task switch {
				   null => TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound)
				 , _ when rooms.Contains(null) => TypedResults.Problem("One or more rooms not found", statusCode: StatusCodes.Status404NotFound)
				 , _ => assignRooms(task, rooms!)
			   };
	}

	private static Results<Ok, ProblemHttpResult> AssignRoomsToTask(CleanControlDb.CleaningTask task, ICollection<Room> rooms) {
		task.Rooms.Clear();
		task.Rooms = rooms;
		return TypedResults.Ok();
	}

	/// <summary>
	///     Retrieves the rooms assigned to a specific task.
	/// </summary>
	/// <param name="taskId">The ID of the task for which to retrieve the assigned rooms.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves the rooms that are assigned to a specific task. It does this by finding the task in the
	///     database using the
	///     provided task ID.
	///     If the task is not found, it returns a <see cref="NotFound" /> result.
	///     If the task is found, it retrieves the rooms assigned to the task, converts them to returnable room objects, and
	///     returns an
	///     <see cref="Ok{T}" /> result with the list of returnable rooms.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok{T}" /> result with the list of
	///     returnable rooms, or a <see cref="NotFound" /> result if the task was not found.
	/// </returns>
	public static Results<Ok<IEnumerable<Schemas.Room>>, NotFound> GetAssignedRooms(Guid taskId, CleancontrolContext db) {
		var task = db.CleaningTasks.Find(taskId);
		if (task is null)
			return TypedResults.NotFound();

		var returnRooms = task
						 .Rooms
						 .ToImmutableArray()
						 .Select(Schemas.Room.FromDbRoom);

		return TypedResults.Ok(returnRooms);
	}

	/// <summary>
	///     Updates a specific task in the database.
	/// </summary>
	/// <param name="taskId">The ID of the task to update.</param>
	/// <param name="task">The task object containing the updated data.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method updates a specific task in the database. It does this by finding the task in the database using the
	///     provided task ID.
	///     If the task is not found, it returns a <see cref="ProblemHttpResult" /> with a message indicating that the task was
	///     not found and a 404 status code.
	///     If the task is found, it updates the task's properties with the properties from the provided task object, saves the
	///     changes, and then returns an <see cref="Ok{T}" /> result with the updated task.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok{T}" /> result with the updated
	///     task, or a <see cref="ProblemHttpResult" /> if the task was not found.
	/// </returns>
	public static Results<Ok<CleaningTask>, ProblemHttpResult> UpdateTask(Guid taskId, CleaningTask task, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		(_, dbTask.Name, dbTask.Description, dbTask.RecurrenceInterval, dbTask.OnCheckout) = task;

		db.SaveChanges();

		var returnTask = CleaningTask.FromDbRoomCleaningTask(dbTask);
		return TypedResults.Ok(returnTask);
	}

	/// <summary>
	///     Retrieves a specific task from the database.
	/// </summary>
	/// <param name="taskId">The ID of the task to retrieve.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves a specific task from the database. It does this by finding the task in the database using the
	///     provided task ID.
	///     If the task is not found, it returns a <see cref="ProblemHttpResult" /> with a message indicating that the task was
	///     not found and a 404 status code.
	///     If the task is found, it creates a returnable task object with the properties from the found task and returns an
	///     <see cref="Ok{T}" /> result with the returnable task.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok{T}" /> result with the
	///     returnable task, or a <see cref="ProblemHttpResult" /> if the task was not found.
	/// </returns>
	public static Results<Ok<CleaningTask>, ProblemHttpResult> GetTask(Guid taskId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem($"Task with ID {taskId} not found", statusCode: StatusCodes.Status404NotFound);

		var returnTask = new CleaningTask(
										  dbTask.Id
										, dbTask.Name
										, dbTask.Description
										, dbTask.RecurrenceInterval
										, dbTask.OnCheckout
										 );

		return TypedResults.Ok(returnTask);
	}

	/// <summary>
	///     Adds a new task to the database.
	/// </summary>
	/// <param name="task">The task object containing the data to create a new task.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method adds a new task to the database. It does this by creating a new task in the database with the
	///     properties from the provided task object.
	///     After the task is created, it returns an <see cref="Ok{T}" /> result with the created task.
	/// </remarks>
	/// <returns>An <see cref="Ok{T}" /> result that contains the created task.</returns>
	public static Ok<CleaningTask> AddTask(CleaningTask task, CleancontrolContext db) {
		var dbTask = new CleanControlDb.CleaningTask {
			Name = task.name
		  , Description = task.description
		  , RecurrenceInterval = task.recurrenceInterval
		  , OnCheckout = task.onCheckout
		};

		var returnTasks = CleaningTask.FromDbRoomCleaningTask(dbTask);

		db.CleaningTasks.Add(dbTask);
		db.SaveChanges();

		return TypedResults.Ok(returnTasks);
	}

	/// <summary>
	///     Retrieves all tasks from the database.
	/// </summary>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves all tasks from the database. It does this by selecting all tasks from the database and
	///     mapping them to the returnable task objects using the CreateReturnTask method.
	/// </remarks>
	/// <returns>An <see cref="Ok{T}" /> result that contains a list of all tasks in the database.</returns>
	public static Ok<IEnumerable<CleaningTask>> GetAllTasks(CleancontrolContext db) =>
		TypedResults.Ok(db.CleaningTasks.Select(CleaningTask.FromDbRoomCleaningTask));
}
