#region

using CleanControlBackend.Schemas;
using CleanControlDb;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;
using Room = CleanControlBackend.Schemas.Room;

#endregion

namespace CleanControlBackend.Routes;

public static class RoomsEndpoints {
	public static void Map(WebApplication app, CleancontrolContext db) {
		var group = app.MapGroup("/rooms");

		group.MapRoomsApi(db).MapGroup("/refills").MapRoomRefillsApi(db).WithOpenApi().WithTags("Room refills");

		group.MapGroup("/tasks").MapRoomTasksApi(db).WithOpenApi().WithTags("Room tasks");
	}

	private static RouteGroupBuilder MapRoomsApi(this RouteGroupBuilder group, CleancontrolContext db) {
		group
		   .MapGet("/", () => TypedResults.Ok<IEnumerable<Room>>(null))
		   .WithDescription("Fetches all rooms")
		   .WithSummary("Get all rooms");

		group
		   .MapPost("/", () => TypedResults.Ok<Room>(null))
		   .WithDescription("Creates a new room")
		   .WithSummary("Create a new room");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<Room>(null))
		   .WithDescription("Fetches a room by its ID")
		   .WithSummary("Get a room by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<Room>(null))
		   .WithDescription("Updates a room by its ID")
		   .WithSummary("Update a room");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok())
		   .WithDescription("Deletes a room by its ID")
		   .WithSummary("Delete a room");

		return group;
	}

	public static RouteGroupBuilder MapRoomRefillsApi(this RouteGroupBuilder group, CleancontrolContext db) {
		group
		   .MapGet("/", () => TypedResults.Ok<IEnumerable<RoomRefill>>(null))
		   .WithDescription("Fetches all room refills")
		   .WithSummary("Get all room refills");

		group
		   .MapPost("/", () => TypedResults.Ok<RoomRefill>(null))
		   .WithDescription("Creates a new room refill")
		   .WithSummary("Create a new room refill");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<RoomRefill>(null))
		   .WithDescription("Fetches a room refill by its ID")
		   .WithSummary("Get a room refill by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<RoomRefill>(null))
		   .WithDescription("Updates a room refill by its ID")
		   .WithSummary("Update a room refill");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok())
		   .WithDescription("Deletes a room refill by its ID")
		   .WithSummary("Delete a room refill");

		return group;
	}

	public static RouteGroupBuilder MapRoomTasksApi(this RouteGroupBuilder group, CleancontrolContext db) {
		group
		   .MapGet("/", () => TypedResults.Ok<IEnumerable<CleaningTask>>(null))
		   .WithDescription("Fetches all room tasks")
		   .WithSummary("Get all room tasks");

		group
		   .MapPost("/", () => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Creates a new room task")
		   .WithSummary("Create a new room task");

		group
		   .MapGet("/{id}", (int id) => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Fetches a room task by its ID")
		   .WithSummary("Get a room task by ID");

		group
		   .MapPut("/{id}", (int id) => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Updates a room task by its ID")
		   .WithSummary("Update a room task");

		group
		   .MapDelete("/{id}", (int id) => TypedResults.Ok<CleaningTask>(null))
		   .WithDescription("Deletes a room task by its ID")
		   .WithSummary("Delete a room task");

		return group;
	}
}
