namespace cleancontrol_backend.routes;

public static class CleaningRunsEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/cleaning_runs").MapCleaningRunsApi().WithOpenApi().WithTags("Cleaning runs");
	}

	private static RouteGroupBuilder MapCleaningRunsApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}
}
