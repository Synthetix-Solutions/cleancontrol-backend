namespace cleancontrol_backend.routes;

public static class CleaningRunsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/cleaning_runs").MapCleaningRunsApi().WithOpenApi().WithTags("Cleaning runs");
	}

	private static RouteGroupBuilder MapCleaningRunsApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok<IEnumerable<Schemas.CleaningRun>>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.CleaningRun>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningRun>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningRun>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningRun>(null));

		return group;
	}
}
