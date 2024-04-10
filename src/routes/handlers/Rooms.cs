#region

using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Room = CleanControlBackend.Schemas.Room;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
///     Handlers for /rooms
/// </summary>
public static class Rooms {
	/// <summary>
	///     Deletes a specific room from the database.
	/// </summary>
	/// <param name="roomId">The ID of the room to delete.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method deletes a specific room from the database. It does this by finding the room in the database using the
	///     provided room ID.
	///     If the room is not found, it returns a <see cref="NotFound" /> result.
	///     If the room is found, it removes the room from the database, saves the changes, and then returns an
	///     <see cref="Ok" /> result.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok" /> result if the room was
	///     successfully deleted, or a <see cref="NotFound" /> result if the room was not found.
	/// </returns>
	public static Results<Ok, NotFound> DeleteRoom(Guid roomId, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(roomId);
		if (dbRoom is null)
			return TypedResults.NotFound();

		db.Rooms.Remove(dbRoom);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	/// <summary>
	///     Updates a specific room in the database.
	/// </summary>
	/// <param name="roomId">The ID of the room to update.</param>
	/// <param name="room">The room object containing the updated data.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method updates a specific room in the database. It does this by finding the room in the database using the
	///     provided room ID.
	///     If the room is not found, it returns a <see cref="NotFound" /> result.
	///     If the room is found, it updates the room's number with the number from the provided room object, saves the
	///     changes, and then returns an <see cref="Ok{T}" /> result with the updated room.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok{T}" /> result with the updated
	///     room, or a <see cref="NotFound" /> result if the room was not found.
	/// </returns>
	public static Results<Ok<Room>, NotFound> UpdateRoom(Guid roomId, Room room, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(roomId);
		if (dbRoom is null)
			return TypedResults.NotFound();

		dbRoom.Number = room.roomNumber;

		var returnRoom = Room.FromDbRoom(dbRoom);

		db.SaveChanges();
		return TypedResults.Ok(returnRoom);
	}

	/// <summary>
	///     Retrieves a specific room from the database.
	/// </summary>
	/// <param name="roomId">The ID of the room to retrieve.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves a specific room from the database. It does this by finding the room in the database using the
	///     provided room ID.
	///     If the room is not found, it returns a <see cref="NotFound" /> result.
	///     If the room is found, it returns an <see cref="Ok{T}" /> result with the room.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok{T}" /> result with the room, or
	///     a <see cref="NotFound" /> result if the room was not found.
	/// </returns>
	public static Results<Ok<Room>, NotFound> GetRoom(Guid roomId, CleancontrolContext db) {
		var dbRoom = db.Rooms.Find(roomId);
		if (dbRoom is null)
			return TypedResults.NotFound();

		var room = Room.FromDbRoom(dbRoom);

		return TypedResults.Ok(room);
	}

	/// <summary>
	///     Creates a new room in the database.
	/// </summary>
	/// <param name="room">The room object containing the data to create a new room.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method creates a new room in the database. It does this by checking if a room with the same number already
	///     exists in the database.
	///     If a room with the same number already exists, it returns a <see cref="Conflict" /> result.
	///     If a room with the same number does not exist, it creates a new room in the database with the number from the
	///     provided room object, saves the changes, and then returns a <see cref="CreatedAtRoute{T}" /> result with the
	///     created room.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either a <see cref="CreatedAtRoute{T}" /> result with
	///     the created room, or a <see cref="Conflict" /> result if a room with the same number already exists.
	/// </returns>
	public static Results<CreatedAtRoute<Room>, Conflict> CreateRoom(Room room, CleancontrolContext db) {
		if (db.Rooms.FirstOrDefault(r => r.Number == room.roomNumber) is not null)
			return TypedResults.Conflict();

		var dbRoom = new CleanControlDb.Room { Number = room.roomNumber };
		db.Rooms.Add(dbRoom);

		var returnRoom = Room.FromDbRoom(dbRoom);

		db.SaveChanges();
		return TypedResults.CreatedAtRoute(
										   returnRoom
										 , "GetRoom"
										 , new { roomId = dbRoom.Id }
										  );
	}

	/// <summary>
	///     Retrieves all rooms from the database.
	/// </summary>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves all rooms from the database. It does this by selecting all rooms from the database and
	///     mapping them to the returnable room objects.
	/// </remarks>
	/// <returns>An <see cref="Ok{T}" /> result that contains a list of all rooms in the database.</returns>
	public static Ok<IEnumerable<Room>> GetAllRooms(CleancontrolContext db) {
		var rooms = db
				   .Rooms
				   .AsEnumerable()
				   .Select(Room.FromDbRoom);

		return TypedResults.Ok(rooms);
	}
}
