#region

using System.Collections.Immutable;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Swashbuckle.AspNetCore.SwaggerGen;
using CleaningRun = CleanControlBackend.Schemas.CleaningRun;

#endregion

namespace CleanControlBackend.Routes;

public static class CleaningRunsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/cleaning_runs")
		   .MapCleaningRunsApi()
		   .WithOpenApi()
		   .WithTags("Cleaning runs");
	}

	private static RouteGroupBuilder MapCleaningRunsApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllProducts)
			 .WithDescription("Fetches all cleaning runs")
			 .WithSummary("Get all cleaning runs");

		group.MapPost("/", CreateCleaningRun)
			 .WithDescription("Creates a new cleaning run")
			 .WithSummary("Create a new cleaning run");

		group.MapGet("/{id}", GetCleaningRun)
			 .WithDescription("Fetches a cleaning run by its ID")
			 .WithSummary("Get a cleaning run by ID");


		group.MapDelete("/{id}", DeleteCleaningRun)
			 .WithDescription("Deletes a cleaning run by its ID")
			 .WithSummary("Delete a cleaning run");

		return group;
	}

	private static Results<ProblemHttpResult, Ok> DeleteCleaningRun(Guid id, CleancontrolContext db) {
		var dbCleaningRun = db.CleaningRuns.Find(id);
		if (dbCleaningRun is null)
			return TypedResults.Problem($"Cleaning run with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		db.CleaningRuns.Remove(dbCleaningRun);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	private static Results<Ok<CleaningRun>, ProblemHttpResult> GetCleaningRun(Guid id, CleancontrolContext db) {
		var dbCleaningRun = db.CleaningRuns.Find(id);
		if (dbCleaningRun is null)
			return TypedResults.Problem($"Cleaning run with ID {id} not found", statusCode: StatusCodes.Status404NotFound);

		var cleaningRun = GetReturnCleaningRun(dbCleaningRun);
		return TypedResults.Ok<CleaningRun>(null);
	}

	private static CleaningRun GetReturnCleaningRun(CleanControlDb.CleaningRun dbCleaningRun) {
		return new CleaningRun(
							   dbCleaningRun.Id
							 , dbCleaningRun.Date
							 , new Schemas.Room(dbCleaningRun.StartingRoom.Id, dbCleaningRun.StartingRoom.Number)
							 , dbCleaningRun.Cleaners.Select(
															 u => new Schemas.User(
																				   u.Id
																				 , u.Name
																				 , u.Username
																				  )
															)
							  );
	}

	private static Results<Ok<CleaningRun>, ProblemHttpResult> CreateCleaningRun(CleaningRun cleaningRun, CleancontrolContext db) {
		var startingRoom = db.Rooms.Find(cleaningRun.startingRoomId);
		if (startingRoom is null)
			return TypedResults.Problem($"Room with ID {cleaningRun.startingRoomId} not found", statusCode: StatusCodes.Status404NotFound);

		if (cleaningRun.cleanerIds is null)
			return TypedResults.Problem("CleanerIds cannot be null", statusCode: StatusCodes.Status400BadRequest);

		var cleaners = cleaningRun.cleanerIds.Select(g => db.Users.Find(g)).ToImmutableArray();
		if (cleaners.Any(c => c is null))
			return TypedResults.Problem("One or more cleaner IDs not found", statusCode: StatusCodes.Status404NotFound);

		var dbCleaningRun = new CleanControlDb.CleaningRun { Date = cleaningRun.date, Cleaners = cleaners, StartingRoom = startingRoom };

		return TypedResults.Ok<CleaningRun>(GetReturnCleaningRun(dbCleaningRun));
	}

	private static Ok<IEnumerable<CleaningRun>> GetAllProducts(CleancontrolContext db) {
		var cleaningRuns = db.CleaningRuns.Select(GetReturnCleaningRun);

		return TypedResults.Ok<IEnumerable<CleaningRun>>(cleaningRuns);
	}
}
