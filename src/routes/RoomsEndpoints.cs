#region

using CleanControlBackend.Routes.Handlers;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

#endregion

namespace CleanControlBackend.Routes;

/// <summary>
///     Contains endpoints for /rooms
/// </summary>
public static class RoomsEndpoints {
	/// <summary>
	///     Maps routes for /rooms
	/// </summary>
	/// <param name="app"></param>
	public static void Map(WebApplication app) {
		var group = app
				   .MapGroup("/rooms")
				   .MapRoomsApi()
				   .AddFluentValidationAutoValidation()
				   .WithOpenApi()
				   .WithTags("Rooms");

		group
		   .MapGroup("/{roomId:guid}/refills")
		   .MapRoomRefillsApi()
		   .AddFluentValidationAutoValidation()
		   .WithOpenApi()
		   .WithTags("Room refills");

		group
		   .MapGroup("/{roomId:guid}/tasks")
		   .MapRoomTasksApi()
		   .AddFluentValidationAutoValidation()
		   .WithOpenApi()
		   .WithTags("Room tasks");
	}

	/// <summary>
	///     Contains endpoints for /rooms/tasks
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	private static RouteGroupBuilder MapRoomTasksApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", RoomTasks.GetDueRoomTasks)
		   .WithDescription("Fetches all room tasks")
		   .WithSummary("Get all room tasks");

		group
		   .MapGet("/{taskId:guid}", RoomTasks.GetRoomTask)
		   .WithDescription("Fetches a room task by its ID")
		   .WithSummary("Get a room task by ID");

		group
		   .MapDelete("/{taskId:guid}", RoomTasks.DeleteRoomTask)
		   .WithDescription("Deletes a room task by its ID")
		   .WithSummary("Delete a room task");

		return group;
	}

	/// <summary>
	///     Maps /rooms
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	private static RouteGroupBuilder MapRoomsApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", Rooms.GetAllRooms)
		   .WithDescription("Fetches all rooms")
		   .WithSummary("Get all rooms");

		group
		   .MapPost("/", Rooms.CreateRoom)
		   .WithDescription("Creates a new room")
		   .WithSummary("Create a new room");

		group
		   .MapGet("/{roomId:guid}", Rooms.GetRoom)
		   .WithDescription("Fetches a room by its ID")
		   .WithSummary("Get a room by ID")
		   .WithName("GetRoom");

		group
		   .MapPut("/{roomId:guid}", Rooms.UpdateRoom)
		   .WithDescription("Updates a room by its ID")
		   .WithSummary("Update a room");

		group
		   .MapDelete("/{roomId:guid}", Rooms.DeleteRoom)
		   .WithDescription("Deletes a room by its ID")
		   .WithSummary("Delete a room");

		return group;
	}

	/// <summary>
	///     Maps /rooms/refills
	/// </summary>
	/// <param name="group"></param>
	/// <returns></returns>
	private static RouteGroupBuilder MapRoomRefillsApi(this RouteGroupBuilder group) {
		group
		   .MapGet("/", RoomRefills.GetAllRoomRefills)
		   .WithDescription("Fetches all room refills")
		   .WithSummary("Get all room refills");

		group
		   .MapPost("/", RoomRefills.AddRoomRefills)
		   .WithDescription("Creates a new room refill")
		   .WithSummary("Create a new room refill");

		group
		   .MapGet("/{refillId:guid}", RoomRefills.GetRoomRefill)
		   .WithDescription("Fetches a room refill by its ID")
		   .WithSummary("Get a room refill by ID");

		group
		   .MapDelete("/{refillId:guid}", RoomRefills.DeleteRoomRefill)
		   .WithDescription("Deletes a room refill by its ID")
		   .WithSummary("Delete a room refill");

		return group;
	}
}
