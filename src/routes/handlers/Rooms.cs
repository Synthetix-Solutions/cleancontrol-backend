#region

using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using CleaningTask = CleanControlBackend.Schemas.CleaningTask;
using Product = CleanControlBackend.Schemas.Product;
using Room = CleanControlBackend.Schemas.Room;

#endregion

namespace CleanControlBackend.Routes.Handlers;

public static class Rooms {
	public static Results<Ok, NotFound> DeleteRoomRefill(Guid refillId, CleancontrolContext db) {
		var dbRoomRefill = db.RoomProductStockRefills.Find(refillId);
		if (dbRoomRefill is null)
			return TypedResults.NotFound();

		db.RoomProductStockRefills.Remove(dbRoomRefill);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	public static Results<NotFound, Ok<RoomRefill>> GetRoomRefill(Guid roomId, Guid refillId, CleancontrolContext db) {
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

		return TypedResults.Ok(returnRefill);
	}

	public static Results<Ok, ProblemHttpResult> AddRoomRefills(Guid roomId, IEnumerable<RoomRefill> refills, CleancontrolContext db) {
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

	public static Ok<IEnumerable<RoomRefill>> GetAllRoomRefills(CleancontrolContext db) {
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


	public static Results<Ok, ProblemHttpResult> DeleteRoomTask(Guid roomId, Guid taskId, CleancontrolContext db) {
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

	public static Results<Ok<CleaningTask>, ProblemHttpResult> GetRoomTask(Guid taskId, Guid roomId, CleancontrolContext db) {
		var dbTask = db.CleaningTasks.Find(taskId);
		if (dbTask is null)
			return TypedResults.Problem("Cleaning task with ID '{taskId}' not found.", statusCode: StatusCodes.Status404NotFound);

		return TypedResults.Ok(CreateReturnRoomCleaningTask(dbTask));
	}

	public static Results<Ok<IEnumerable<CleaningTask>>, ProblemHttpResult> GetDueRoomTasks(Guid roomId, CleancontrolContext db) {
		var room = db.Rooms.Find(roomId);
		if (room is null)
			return TypedResults.Problem("Room with ID '{roomId}' not found.", statusCode: StatusCodes.Status404NotFound);

		var dbTasks = room.CleaningTasks.Where(ct => ct.GetNextDueDate() <= DateOnly.FromDateTime(DateTime.UtcNow));
		var tasks = dbTasks.Select(CreateReturnRoomCleaningTask);

		return TypedResults.Ok(tasks);
	}

	public static CleaningTask CreateReturnRoomCleaningTask(CleanControlDb.CleaningTask dbTask) =>
		new(
			dbTask.Id
		  , dbTask.Name
		  , dbTask.Description
		  , dbTask.RecurrenceInterval
		  , dbTask.OnCheckout
		   );


	public static Results<Ok, NotFound> DeleteRoom(Guid roomId, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(roomId);
		if (dbRoom is null)
			return TypedResults.NotFound();

		db.Rooms.Remove(dbRoom);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	public static Results<Ok<Room>, NotFound> UpdateRoom(Guid roomId, Room room, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(roomId);
		if (dbRoom is null)
			return TypedResults.NotFound();

		dbRoom.Number = room.roomNumber;

		var returnRoom = new Room(dbRoom.Id, dbRoom.Number);

		db.SaveChanges();
		return TypedResults.Ok(returnRoom);
	}

	public static Results<Ok<Room>, NotFound> GetRoom(Guid roomId, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(roomId);
		if (dbRoom is null)
			return TypedResults.NotFound();

		var room = new Room(dbRoom.Id, dbRoom.Number);

		return TypedResults.Ok(room);
	}

	public static Results<CreatedAtRoute<Room>, Conflict> CreateRoom(Room room, CleancontrolContext db) {
		if (db.Rooms.FirstOrDefault(r => r.Number == room.roomNumber) is not null)
			return TypedResults.Conflict();

		var dbRoom = new CleanControlDb.Room { Number = room.roomNumber };
		db.Rooms.Add(dbRoom);

		var returnRoom = new Room(dbRoom.Id, dbRoom.Number);

		db.SaveChanges();
		return TypedResults.CreatedAtRoute(
										   returnRoom
										 , "GetRoom"
										 , new { roomId = dbRoom.Id }
										  );
	}

	public static Ok<IEnumerable<Room>> GetAllRooms(CleancontrolContext db) {
		var rooms = db.Rooms.Select(r => new Room(r.Id, r.Number));

		return TypedResults.Ok<IEnumerable<Room>>(rooms);
	}
}
