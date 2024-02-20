namespace cleancontrol_backend.routes;

public static class TasksEndpoints {
	public static void Map(WebApplication app) {
		app.MapGroup("/tasks").MapTasksApi().WithOpenApi().WithTags("Tasks");
	}

	private static RouteGroupBuilder MapTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}
}
