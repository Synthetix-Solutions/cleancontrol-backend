#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningRun = CleanControlBackend.Schemas.CleaningRun;

#endregion

namespace CleanControlBackend.Routes;

public static class CleaningRunsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/cleaning_runs").MapCleaningRunsApi().WithOpenApi().WithTags("Cleaning runs");
	}

	private static RouteGroupBuilder MapCleaningRunsApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", GetAllProducts)
		   .WithDescription("Fetches all cleaning runs")
		   .WithSummary("Get all cleaning runs");

		group
		   .MapPost("/", CreateProduct)
		   .WithDescription("Creates a new cleaning run")
		   .WithSummary("Create a new cleaning run");

		group
		   .MapGet("/{id}", GetProduct)
		   .WithDescription("Fetches a cleaning run by its ID")
		   .WithSummary("Get a cleaning run by ID");

		group
		   .MapPut("/{id}", UpdateProduct)
		   .WithDescription("Updates a cleaning run by its ID")
		   .WithSummary("Update a cleaning run");

		group
		   .MapDelete("/{id}", DeleteProduct)
		   .WithDescription("Deletes a cleaning run by its ID")
		   .WithSummary("Delete a cleaning run");

		return group;
	}

	private static Ok<CleaningRun> DeleteProduct(Guid id) {
		return TypedResults.Ok<CleaningRun>(null);
	}

	private static Ok<CleaningRun> UpdateProduct(Guid id) {
		return TypedResults.Ok<CleaningRun>(null);
	}

	private static Ok<CleaningRun> GetProduct(Guid id) {
		return TypedResults.Ok<CleaningRun>(null);
	}

	private static Ok<CleaningRun> CreateProduct() {
		return TypedResults.Ok<CleaningRun>(null);
	}

	private static Ok<IEnumerable<CleaningRun>> GetAllProducts() {
		return TypedResults.Ok<IEnumerable<CleaningRun>>(null);
	}
}
