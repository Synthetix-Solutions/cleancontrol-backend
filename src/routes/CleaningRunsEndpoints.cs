#region

using CleanControlBackend.Routes.Handlers;
using CleanControlBackend.Schemas;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

#endregion

namespace CleanControlBackend.Routes;

/// <summary>
///     Contains endpoints for /cleaningRuns
/// </summary>
public static class CleaningRunsEndpoints {
	/// <summary>
	///     Maps routes for /cleaningRuns
	/// </summary>
	/// <param name="app"></param>
	public static void Map(WebApplication app) {
		app
		   .MapGroup("/cleaning_runs")
		   .MapCleaningRunsApi()
		   .RequireAuthorization(Policies.AdminOrCleanerOnly)
		   .AddFluentValidationAutoValidation()
		   .WithOpenApi()
		   .WithTags("Cleaning runs");
	}

	private static RouteGroupBuilder MapCleaningRunsApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", CleaningRuns.GetAllCleaningRuns)
		   .WithDescription("Fetches all cleaning runs")
		   .WithSummary("Get all cleaning runs");


		group
		   .MapGet("/{cleaningRunId:guid}", CleaningRuns.GetCleaningRun)
		   .WithName("GetCleaningRun")
		   .WithDescription("Fetches a cleaning run by its ID")
		   .WithSummary("Get a cleaning run by ID");

		group
		   .MapPost("/", CleaningRuns.CreateCleaningRun)
		   .WithDescription("Creates a new cleaning run")
		   .WithSummary("Create a new cleaning run");

		group
		   .MapDelete("/{cleaningRunId:guid}", CleaningRuns.DeleteCleaningRun)
		   .WithDescription("Deletes a cleaning run by its ID")
		   .WithSummary("Delete a cleaning run");

		group
		   .MapGet("/{cleaningRunId:guid}/nextRoom", CleaningRuns.GetNextRoom)
		   .WithDescription("Fetches the next room to clean")
		   .WithSummary("Get the next room to clean");

		group
		   .MapPatch("/{cleaningRunId:guid}/phase", CleaningRuns.UpdateCleaningRunPhase)
		   .WithDescription("Fetches the next room to clean")
		   .WithSummary("Get the next room to clean");
		return group;
	}
}
