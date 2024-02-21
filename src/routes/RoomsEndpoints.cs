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
		group.MapGet("/", () => TypedResults.Ok<IEnumerable<Schemas.Room>>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.Room>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.Room>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.Room>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok());

		return group;
	}

	public static RouteGroupBuilder MapRoomRefillsApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok<IEnumerable<Schemas.RoomRefill>>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.RoomRefill>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.RoomRefill>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.RoomRefill>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok());

		return group;
	}

	public static RouteGroupBuilder MapRoomTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", () => TypedResults.Ok<IEnumerable<Schemas.CleaningTask>>(null));
		group.MapPost("/", () => TypedResults.Ok<Schemas.CleaningTask>(null));

		group.MapGet("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningTask>(null));
		group.MapPut("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningTask>(null));
		group.MapDelete("/{id}", (int id) => TypedResults.Ok<Schemas.CleaningTask>(null));

		return group;
	}
}
