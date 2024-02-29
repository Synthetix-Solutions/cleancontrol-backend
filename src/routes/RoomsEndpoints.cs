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
		group.MapGet("/", GetAllRooms)
			 .WithDescription("Fetches all rooms")
			 .WithSummary("Get all rooms");

		group.MapPost("/", CreateRoom)
			 .WithDescription("Creates a new room")
			 .WithSummary("Create a new room");

		group.MapGet("/{roomId}", GetRoom)
			 .WithDescription("Fetches a room by its ID")
			 .WithSummary("Get a room by ID")
			 .WithName("GetRoom");

		group.MapPut("/{roomId}", UpdateRoom)
			 .WithDescription("Updates a room by its ID")
			 .WithSummary("Update a room");

		group.MapDelete("/{roomId}", DeleteRoom)
			 .WithDescription("Deletes a room by its ID")
			 .WithSummary("Delete a room");

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
		return TypedResults.Ok(returnRoom);
	}

	private static Results<Ok<Room>, NotFound> GetRoom(Guid id, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(id);
		if (dbRoom is null)
			return TypedResults.NotFound();

		var room = new Room(dbRoom.Id, dbRoom.Number);

		return TypedResults.Ok(room);
	}

	private static Results<CreatedAtRoute<Room>, Conflict> CreateRoom(Room room, CleancontrolContext db) {
		if (db.Rooms.FirstOrDefault(r => r.Number == room.roomNumber) is not null)
			return TypedResults.Conflict();

		var dbRoom = new CleanControlDb.Room { Number = room.roomNumber, };
		db.Rooms.Add(dbRoom);

		var returnRoom = new Room(dbRoom.Id, dbRoom.Number);

		db.SaveChanges();
		return TypedResults.CreatedAtRoute(
										   returnRoom
										 , "GetRoom"
										 , new { roomId = dbRoom.Id }
										  );
	}

	private static Ok<IEnumerable<Room>> GetAllRooms(CleancontrolContext db) {
		var rooms = db.Rooms.Select(r => new Room(r.Id, r.Number));

		return TypedResults.Ok<IEnumerable<Room>>(rooms);
	}

	public static RouteGroupBuilder MapRoomRefillsApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetAllRoomRefills)
			 .WithDescription("Fetches all room refills")
			 .WithSummary("Get all room refills");

		group.MapPost("/", AddRoomRefills)
			 .WithDescription("Creates a new room refill")
			 .WithSummary("Create a new room refill");

		group.MapGet("/{refillId}", GetRoomRefill)
			 .WithDescription("Fetches a room refill by its ID")
			 .WithSummary("Get a room refill by ID");

		group.MapDelete("/{refillId}", DeleteRoomRefill)
			 .WithDescription("Deletes a room refill by its ID")
			 .WithSummary("Delete a room refill");

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
										, dbRefill.Amount
										 );

		return TypedResults.Ok<RoomRefill>(returnRefill);
	}

	private static Results<Ok, ProblemHttpResult> AddRoomRefills(Guid roomId, IEnumerable<RoomRefill> refills, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		try {
			var refillSelector = (RoomRefill r) => {
									 var product = db.Products.Find(r.id)
												?? throw new InvalidOperationException($"Product with ID '{r.id}' not found.");

									 var roomProduct = db.RoomProducts.FirstOrDefault(rp => rp.Room == room && rp.Product == product)
													?? throw new InvalidOperationException(
																						   $"Room product association for room ID '{r.id}' and product ID '{product.Id}' not found."
																						  );

									 return new RoomProductRefill { RoomProduct = roomProduct, Amount = r.quantity, Date = DateTime.UtcNow };
								 };

			var dbRefills = refills.Select(refillSelector);
			db.RoomProductStockRefills.AddRange(dbRefills);

			db.SaveChanges();
		} catch (InvalidOperationException e) {
			return TypedResults.Problem(e.Message, statusCode: StatusCodes.Status404NotFound);
		}

		return TypedResults.Ok();
	}

	private static Ok<IEnumerable<RoomRefill>> GetAllRoomRefills(CleancontrolContext db) {
		var refills = db.RoomProductStockRefills.Select(
														r => new RoomRefill(
																			new Product(
																						r.RoomProduct.Product.Id
																					  , r.RoomProduct.Product.Name
																					  , r.RoomProduct.Product.InventoryQuantity
																					   )
																		  , r.Amount
																		   )
													   );


		return TypedResults.Ok<IEnumerable<RoomRefill>>(refills);
	}

	public static RouteGroupBuilder MapRoomTasksApi(this RouteGroupBuilder group) {
		group.MapGet("/", GetDueRoomTasks)
			 .WithDescription("Fetches all room tasks")
			 .WithSummary("Get all room tasks");

		group.MapGet("/{id}", GetRoomTask)
			 .WithDescription("Fetches a room task by its ID")
			 .WithSummary("Get a room task by ID");

		group.MapDelete("/{id}", DeleteRoomTask)
			 .WithDescription("Deletes a room task by its ID")
			 .WithSummary("Delete a room task");

		return group;
	}

	private static Results<Ok, ProblemHttpResult> DeleteRoomTask(Guid roomId, Guid taskId, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var cleaningTask = db.CleaningTasks.Find(taskId);
		if (cleaningTask is null)
			return TypedResults.Problem($"Cleaning task with ID '{taskId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var roomCleanup = new RoomCleanup { CleaningTask = cleaningTask, Room = room, Date = DateTime.UtcNow };
		db.RoomCleanups.Add(roomCleanup);
		db.SaveChanges();

		return TypedResults.Ok();
	}

	private static Results<Ok<CleaningTask>, ProblemHttpResult> GetRoomTask(Guid taskId, Guid roomId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem("Cleaning task with ID '{taskId}' not found.", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok(CreateReturnRoomCleaningTask(dbTask));
	}

	private static Results<Ok<IEnumerable<CleaningTask>>, ProblemHttpResult> GetDueRoomTasks(Guid roomId, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var dbTasks = room.CleaningTasks.Where(ct => ct.GetNextDueDate() <= DateOnly.FromDateTime(DateTime.UtcNow));
		var tasks = dbTasks.Select(CreateReturnRoomCleaningTask);

		return TypedResults.Ok(tasks);
	}

	private static CleaningTask CreateReturnRoomCleaningTask(CleanControlDb.CleaningTask dbTask) =>
		new(
			dbTask.Id
		  , dbTask.Name
		  , dbTask.Description
		  , dbTask.RecurrenceInterval
		  , dbTask.OnCheckout
		   );
}
