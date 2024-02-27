#region

using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;
using Product = CleanControlBackend.Schemas.Product;
using Room = CleanControlBackend.Schemas.Room;

#endregion

namespace CleanControlBackend.Routes;

public static class RoomsEndpoints {
	public static void Map(WebApplication app) {
		var group = app.MapGroup("/rooms");

		group.MapRoomsApi().MapGroup("/refills").MapRoomRefillsApi().WithOpenApi().WithTags("Room refills");

		group.MapGroup("/tasks").MapRoomTasksApi().WithOpenApi().WithTags("Room tasks");
	}

	private static RouteGroupBuilder MapRoomsApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllRooms).WithDescription("Fetches all rooms").WithSummary("Get all rooms");

		group.MapPost("/", CreateRoom).WithDescription("Creates a new room").WithSummary("Create a new room");

		group.MapGet("/{roomId}", GetRoom).WithDescription("Fetches a room by its ID").WithSummary("Get a room by ID").WithName("GetRoom");

		group.MapPut("/{roomId}", UpdateRoom).WithDescription("Updates a room by its ID").WithSummary("Update a room");

		group.MapDelete("/{roomId}", DeleteRoom).WithDescription("Deletes a room by its ID").WithSummary("Delete a room");

		return group;
	}

	private static Results<Ok, NotFound> DeleteRoom(Guid id, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(id);
		if (dbRoom is null)
			return TypedResults.NotFound();

		db.Rooms.Remove(dbRoom);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	private static Results<Ok<Room>, NotFound> UpdateRoom(Guid id, Room room, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(id);
		if (dbRoom is null)
			return TypedResults.NotFound();

		dbRoom.Number = room.roomNumber;

		var returnRoom = new Room(dbRoom.Id, dbRoom.Number);

		db.SaveChanges();
		return TypedResults.Ok<Room>(returnRoom);
	}

	private static Results<Ok<Room>, NotFound> GetRoom(Guid id, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(id);
		if (dbRoom is null)
			return TypedResults.NotFound();

		var room = new Room(dbRoom.Id, dbRoom.Number);

		return TypedResults.Ok<Room>(room);
	}

	private static Results<CreatedAtRoute<Room>, Conflict> CreateRoom(Room room, CleancontrolContext db) {
		if (db.Rooms.FirstOrDefault(r => r.Number == room.roomNumber) is not null)
			return TypedResults.Conflict();

		var dbRoom = new CleanControlDb.Room() { Number = room.roomNumber };
		db.Rooms.Add(dbRoom);

		var returnRoom = new Room(dbRoom.Id, dbRoom.Number);

		db.SaveChanges();
		return TypedResults.CreatedAtRoute<Room>(
												 returnRoom
											   , "GetRoom"
											   , new { id = dbRoom.Id }
												);
	}

	private static Ok<IEnumerable<Room>> GetAllRooms(CleancontrolContext db) {
		var rooms = db.Rooms.Select(r => new Room(r.Id, r.Number));

		return TypedResults.Ok<IEnumerable<Room>>(rooms);
	}

	public static RouteGroupBuilder MapRoomRefillsApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllRoomRefills).WithDescription("Fetches all room refills").WithSummary("Get all room refills");

		group.MapPost("/", AddRoomRefill).WithDescription("Creates a new room refill").WithSummary("Create a new room refill");

		group.MapGet("/{refillId}", GetRoomRefill).WithDescription("Fetches a room refill by its ID").WithSummary("Get a room refill by ID");

		group.MapDelete("/{refillId}", DeleteRoomRefill).WithDescription("Deletes a room refill by its ID").WithSummary("Delete a room refill");

		return group;
	}

	private static Results<Ok, NotFound> DeleteRoomRefill(Guid id, CleancontrolContext db) {
		var dbRoomRefill = db.RoomProductStockRefills.Find(id);
		if (dbRoomRefill is null)
			return TypedResults.NotFound();

		db.RoomProductStockRefills.Remove(dbRoomRefill);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	private static Results<NotFound, Ok<RoomRefill>> GetRoomRefill(Guid roomId, Guid refillId, CleancontrolContext db) {
		var dbRefill = db.RoomProductStockRefills.Find(refillId);
		if (dbRefill is null)
			return TypedResults.NotFound();

		var returnRefill = new RoomRefill(
										  new Product(
													  dbRefill.RoomProduct.Product.Id
													, dbRefill.RoomProduct.Product.Name
													, dbRefill.RoomProduct.Product.InventoryQuantity
													 )
										, null
										, dbRefill.Amount
										 );

		return TypedResults.Ok<RoomRefill>(null);
	}

	private static Ok<RoomRefill> AddRoomRefill() {
		return TypedResults.Ok<RoomRefill>(null);
	}

	private static Ok<IEnumerable<RoomRefill>> GetAllRoomRefills() {
		return TypedResults.Ok<IEnumerable<RoomRefill>>(null);
	}

	public static RouteGroupBuilder MapRoomTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllRoomTasks).WithDescription("Fetches all room tasks").WithSummary("Get all room tasks");

		group.MapPost("/", CreateRoomTask).WithDescription("Creates a new room task").WithSummary("Create a new room task");

		group.MapGet("/{id}", GetRoomTask).WithDescription("Fetches a room task by its ID").WithSummary("Get a room task by ID");

		group.MapPut("/{id}", UpdateRoomTask).WithDescription("Updates a room task by its ID").WithSummary("Update a room task");

		group.MapDelete("/{id}", DeleteRoomTask).WithDescription("Deletes a room task by its ID").WithSummary("Delete a room task");

		return group;
	}

	private static Ok<CleaningTask> DeleteRoomTask(Guid id) {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<CleaningTask> UpdateRoomTask(Guid id) {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<CleaningTask> GetRoomTask(Guid id) {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<CleaningTask> CreateRoomTask() {
		return TypedResults.Ok<CleaningTask>(null);
	}

	private static Ok<IEnumerable<CleaningTask>> GetAllRoomTasks() {
		return TypedResults.Ok<IEnumerable<CleaningTask>>(null);
	}
}
