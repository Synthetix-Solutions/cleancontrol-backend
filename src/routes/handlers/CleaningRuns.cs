#region

using System.Collections.Immutable;
using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using CleaningRun = CleanControlBackend.Schemas.CleaningRun;
using Room = CleanControlBackend.Schemas.Room;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
///     Handlers for /cleaningRuns
/// </summary>
public static class CleaningRuns {
	/// <summary>
	///     Deletes a cleaning run by its ID
	/// </summary>
	/// <param name="cleaningRunId">ID of the cleaning run</param>
	/// <param name="db">Database context</param>
	/// <returns>
	///     <see cref="Ok" /> if the cleaning run got deleted, else a <see cref="ProblemHttpResult" /> containing error
	///     details.
	/// </returns>
	public static Results<ProblemHttpResult, Ok> DeleteCleaningRun(Guid cleaningRunId, CleancontrolContext db) {
		var dbCleaningRun = db.CleaningRuns.Find(cleaningRunId);
		if (dbCleaningRun is null)
			return TypedResults.Problem($"Cleaning run with ID {cleaningRunId} not found", statusCode: StatusCodes.Status404NotFound);

		db.CleaningRuns.Remove(dbCleaningRun);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	/// <summary>
	///     Gets a cleaning run by its ID
	/// </summary>
	/// <param name="cleaningRunId">ID of the cleaning run</param>
	/// <param name="db">Database context</param>
	/// <returns>
	///     <see cref="Ok{CleaningRun}" /> with the cleaning run data, else a <see cref="ProblemHttpResult" /> containing
	///     error details.
	/// </returns>
	public static Results<Ok<CleaningRun>, ProblemHttpResult> GetCleaningRun(Guid cleaningRunId, CleancontrolContext db) {
		var dbCleaningRun = db.CleaningRuns.Find(cleaningRunId);

		if (dbCleaningRun is null)
			return TypedResults.Problem($"Cleaning run with ID {cleaningRunId} not found", statusCode: StatusCodes.Status404NotFound);

		var cleaningRun = GetReturnCleaningRun(dbCleaningRun);

		return TypedResults.Ok(cleaningRun);
	}


	/// <summary>
	///     Converts a database cleaning run to a return cleaning run
	/// </summary>
	/// <param name="dbCleaningRun">Database cleaning run</param>
	/// <returns>Return cleaning run</returns>
	public static CleaningRun GetReturnCleaningRun(CleanControlDb.CleaningRun dbCleaningRun) =>
		new(
			dbCleaningRun.Id
		  , dbCleaningRun.Date
		  , Room.FromDbRoom(dbCleaningRun.StartingRoom)
		  , dbCleaningRun.Cleaners.Select(
										  u => new User(
														u.Id
													  , u
														   .Name
													  , u
														   .Email
														!
													   )
										 )
		  , dbCleaningRun.Phase
		   );

	/// <summary>
	///     Creates a new cleaning run
	/// </summary>
	/// <param name="cleaningRun">Cleaning run data</param>
	/// <param name="db">Database context</param>
	/// <returns>
	///     <see cref="Ok{CleaningRun}" /> with the created cleaning run data, else a <see cref="ProblemHttpResult" />
	///     containing error details.
	/// </returns>
	public static Results<CreatedAtRoute<CleaningRun>, ProblemHttpResult> CreateCleaningRun(CleaningRun cleaningRun, CleancontrolContext db) {
		var startingRoom = db.Rooms.Find(cleaningRun.startingRoomId);

		if (startingRoom is null)
			return TypedResults.Problem($"Room with ID {cleaningRun.startingRoomId} not found", statusCode: StatusCodes.Status404NotFound);

		if (cleaningRun.cleanerIds is null)
			return TypedResults.Problem("CleanerIds cannot be null", statusCode: StatusCodes.Status400BadRequest);

		var cleaners = cleaningRun
					  .cleanerIds
					  .Select(g => db.Users.Find(g))
					  .ToImmutableArray();

		// If any of the cleaners do not exist, return a 404 error
		if (cleaners.Any(c => c is null))
			return TypedResults.Problem("One or more cleaner IDs not found", statusCode: StatusCodes.Status404NotFound);

		var dbCleaningRun = new CleanControlDb.CleaningRun { Cleaners = cleaners, StartingRoom = startingRoom };

		db.CleaningRuns.Add(dbCleaningRun);
		db.SaveChanges();

		return TypedResults.CreatedAtRoute(
										   GetReturnCleaningRun(dbCleaningRun)
										 , "GetCleaningRun"
										 , new { cleaningRunId = dbCleaningRun.Id }
										  );
	}

	/// <summary>
	///     Retrieves all cleaning runs.
	/// </summary>
	/// <param name="db">The database context.</param>
	/// <param name="context">
	///     The HttpContext object that encapsulates all HTTP-specific information about an individual HTTP
	///     request.
	/// </param>
	/// <param name="userManager">The UserManager object that provides the APIs for managing user in a persistence store.</param>
	/// <remarks>
	///     This method retrieves all cleaning runs that the current user has access to. It does this by getting the current
	///     user and their role,
	///     and then filtering the cleaning runs based on the user's role. If the user is an admin, they have access to all
	///     cleaning runs.
	///     If the user is a cleaner, they only have access to cleaning runs that are not finished and that they are a part of.
	///     The cleaning runs are then converted to return cleaning runs and returned.
	/// </remarks>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation. The task result contains an
	///     <see cref="Ok{T}" /> result with a list of cleaning runs.
	/// </returns>
	public static async Task<Ok<IEnumerable<CleaningRun>>> GetAllCleaningRuns(CleancontrolContext db
																			, HttpContext context
																			, UserManager<CleanControlUser> userManager
	) {
		var currentUser = userManager.GetUserAsync(context.User)
									 .Result!;
		var currentUserRole = await currentUser.GetRole(userManager);

		var cleaningRuns = db
						  .CleaningRuns
						  .ToList()
						  .Where(
								 cr => (cr, currentUserRole) switch {
										   (_, Role.Admin) => true
										 , ({ Phase: not CleaningRunPhase.Finished, Cleaners: { } cleaners }, Role.Cleaner) =>
											   cleaners.Contains(currentUser)
										 , _ => false
									   }
								)
						  .Select(GetReturnCleaningRun);

		return TypedResults.Ok(cleaningRuns);
	}

	/// <summary>
	///     Retrieves the next room to be cleaned in a cleaning run.
	/// </summary>
	/// <param name="cleaningRunId">The ID of the cleaning run.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves the next room to be cleaned in a cleaning run. It does this by finding the cleaning run in
	///     the database,
	///     getting the starting room of the cleaning run, and then finding the next room in the database that has a cleaning
	///     task due.
	///     If no such room is found, it returns a NotFound result.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2, T3}" /> object that contains either an <see cref="Ok{T}" /> result with the next
	///     room to be cleaned, a <see cref="ProblemHttpResult" /> with an error message, or a <see cref="NotFound" /> result
	///     if no next room is found.
	/// </returns>
	public static Results<Ok<Room>, ProblemHttpResult, NotFound> GetNextRoom(Guid cleaningRunId, CleancontrolContext db) {
		var dbCleaningRun = db.CleaningRuns.Find(cleaningRunId);
		if (dbCleaningRun is null)
			return TypedResults.Problem($"Cleaning run with ID {cleaningRunId} not found", statusCode: StatusCodes.Status404NotFound);

		var startingRoom = dbCleaningRun.StartingRoom;
		var nextRoom = db
					  .Rooms
					   // ReSharper disable once StringCompareIsCultureSpecific.1 - Doesn't translate to SQL
					  .OrderBy(r => string.Compare(startingRoom.Number, r.Number) + "_" + r.Number)
					  .ToList()
					  .FirstOrDefault(r => r.CleaningTasks.Any(ct => ct.GetNextDueDate(r) <= DateOnly.FromDateTime(DateTime.UtcNow)));

		if (nextRoom is null)
			return TypedResults.NotFound();

		var returnRoom = Room.FromDbRoom(nextRoom);

		return TypedResults.Ok(returnRoom);
	}

	/// <summary>
	///     Updates the phase of a cleaning run.
	/// </summary>
	/// <param name="cleaningRunId">The ID of the cleaning run to update.</param>
	/// <param name="phase">The new phase for the cleaning run.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method updates the phase of a cleaning run. It does this by finding the cleaning run in the database,
	///     updating its phase, and then saving the changes to the database.
	///     If the cleaning run is not found, it returns a <see cref="ProblemHttpResult" /> with a 404 status code.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2, T3}" /> object that contains either an <see cref="Ok{T}" /> result with the
	///     updated cleaning run, a <see cref="ProblemHttpResult" /> with an error message, or a <see cref="NotFound" /> result
	///     if the cleaning run is not found.
	/// </returns>
	public static Results<ProblemHttpResult, Ok<CleaningRun>, NotFound> UpdateCleaningRunPhase(
		Guid cleaningRunId
	  , CleaningRunPhase phase
	  , CleancontrolContext db
	) {
		var dbCleaningRun = db.CleaningRuns.Find(cleaningRunId);
		if (dbCleaningRun is null)
			return TypedResults.Problem($"Cleaning run with ID {cleaningRunId} not found", statusCode: StatusCodes.Status404NotFound);

		dbCleaningRun.Phase = phase;

		var returnCleaningRun = GetReturnCleaningRun(dbCleaningRun);

		db.SaveChanges();

		return TypedResults.Ok(returnCleaningRun);
	}
}
