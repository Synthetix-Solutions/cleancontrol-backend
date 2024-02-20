namespace cleancontrol_backend.routes;

public static class RoomsEndpoints {
	public static void Map(WebApplication app) {
		var group = app.MapGroup("/rooms");

		group.MapRoomsApi()
			 .MapGroup("/refills")
			 .MapRoomRefillsApi()
			 .WithOpenApi()
			 .WithTags("Room refills");

		group.MapGroup("/tasks")
			 .MapRoomTasksApi()
			 .WithOpenApi()
			 .WithTags("Room tasks");
	}

	private static RouteGroupBuilder MapRoomsApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}

	public static RouteGroupBuilder MapRoomRefillsApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}

	public static RouteGroupBuilder MapRoomTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => "");
		group.MapPost("/", () => "");

		group.MapGet("/{id}", (int id) => id);
		group.MapPut("/{id}", (int id) => id);
		group.MapDelete("/{id}", (int id) => id);

		return group;
	}
}
