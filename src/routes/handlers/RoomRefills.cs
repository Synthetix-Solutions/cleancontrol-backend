using CleanControlBackend.Schemas;
using CleanControlDb;
using Microsoft.AspNetCore.Http.HttpResults;
using Product = CleanControlBackend.Schemas.Product;

namespace CleanControlBackend.Routes.Handlers;

public static class RoomRefills {
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
}
