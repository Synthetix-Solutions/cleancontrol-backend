#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Room = CleanControlBackend.Schemas.Room;

#endregion

namespace CleanControlBackend.Routes.Handlers;

public static class Rooms {
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

	public static Room GetReturnRoom(CleanControlDb.Room room) => new(room.Id, room.Number);
}
