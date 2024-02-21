#region

using CleanControlDb;
using CleaningRun = CleanControlBackend.Schemas.CleaningRun;

#endregion

namespace CleanControlBackend.Routes;

public static class CleaningRunsEndpoints {
	public static void Map(WebApplication app, CleancontrolContext db) {
		app.MapGroup("/cleaning_runs").MapCleaningRunsApi(db).WithOpenApi().WithTags("Cleaning runs");
	}

	private static RouteGroupBuilder MapCleaningRunsApi(this RouteGroupBuilder group, CleancontrolContext db) {
		group
		   .MapGet("/", () => TypedResults.Ok<IEnumerable<CleaningRun>>(null))
		   .WithDescription("Fetches all cleaning runs")
		   .WithSummary("Get all cleaning runs");

		group
		   .MapPost("/", () => TypedResults.Ok<CleaningRun>(null))
		   .WithDescription("Creates a new cleaning run")
		   .WithSummary("Create a new cleaning run");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<CleaningRun>(null))
		   .WithDescription("Fetches a cleaning run by its ID")
		   .WithSummary("Get a cleaning run by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<CleaningRun>(null))
		   .WithDescription("Updates a cleaning run by its ID")
		   .WithSummary("Update a cleaning run");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok<CleaningRun>(null))
		   .WithDescription("Deletes a cleaning run by its ID")
		   .WithSummary("Delete a cleaning run");

		return group;
	}
}
