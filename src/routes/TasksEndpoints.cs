namespace cleancontrol_backend.routes;

public static class TasksEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/tasks").MapTasksApi().WithOpenApi().WithTags("Tasks");
	}

	private static RouteGroupBuilder MapTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok<Schemas.CleaningTask[]>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.CleaningTask>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningTask>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningTask>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok());

		return group;
	}
}
