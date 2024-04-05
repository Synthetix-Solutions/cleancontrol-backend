#region

using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Product = CleanControlBackend.Schemas.Product;

#endregion

namespace CleanControlBackend.Routes.Handlers;

/// <summary>
///     Room refills handlers
/// </summary>
public static class RoomRefills {
	/// <summary>
	///     Deletes a room refill from the database.
	/// </summary>
	/// <param name="refillId">The ID of the room refill to delete.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method deletes a room refill from the database. It does this by finding the room refill in the database using
	///     the provided refill ID.
	///     If the room refill is not found, it returns a <see cref="NotFound" /> result.
	///     If the room refill is found, it removes it from the database and saves the changes, then returns an
	///     <see cref="Ok" /> result.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok" /> result if the room refill
	///     was successfully deleted, or a <see cref="NotFound" /> result if the room refill was not found.
	/// </returns>
	public static Results<Ok, NotFound> DeleteRoomRefill(Guid refillId, CleancontrolContext db) {
		var dbRoomRefill = db.RoomProductStockRefills.Find(refillId);
		if (dbRoomRefill is null)
			return TypedResults.NotFound();

		db.RoomProductStockRefills.Remove(dbRoomRefill);
		db.SaveChanges();
		return TypedResults.Ok();
	}

	/// <summary>
	///     Retrieves a specific room refill from the database.
	/// </summary>
	/// <param name="roomId">The ID of the room associated with the refill.</param>
	/// <param name="refillId">The ID of the room refill to retrieve.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves a specific room refill from the database. It does this by finding the room refill in the
	///     database using the provided room ID and refill ID.
	///     If the room refill is not found, it returns a <see cref="NotFound" /> result.
	///     If the room refill is found, it returns an <see cref="Ok{T}" /> result with the room refill.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok{T}" /> result with the room
	///     refill, or a <see cref="NotFound" /> result if the room refill was not found.
	/// </returns>
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

	/// <summary>
	///     Adds a list of room refills to the database.
	/// </summary>
	/// <param name="roomId">The ID of the room associated with the refills.</param>
	/// <param name="refills">The list of room refills to be added to the database.</param>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method adds a list of room refills to the database. It does this by finding the room in the database using the
	///     provided room ID,
	///     and then adding each refill in the list to the database associated with the room.
	///     If the room is not found, it returns a <see cref="ProblemHttpResult" /> with a 404 status code and an error
	///     message.
	///     If the room is found and the refills are successfully added, it saves the changes to the database and returns an
	///     <see cref="Ok" /> result.
	/// </remarks>
	/// <returns>
	///     A <see cref="Results{T1, T2}" /> object that contains either an <see cref="Ok" /> result if the refills were
	///     successfully added, or a <see cref="ProblemHttpResult" /> with an error message if the room was not found or there
	///     was an error adding the refills.
	/// </returns>
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

	/// <summary>
	///     Retrieves all room refills from the database.
	/// </summary>
	/// <param name="db">The database context.</param>
	/// <remarks>
	///     This method retrieves all room refills from the database. It does this by selecting all room refills from the
	///     database and returning them as a list.
	///     Each room refill in the list contains the associated product information and the refill amount.
	/// </remarks>
	/// <returns>An <see cref="Ok{T}" /> result with the list of all room refills.</returns>
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
}
